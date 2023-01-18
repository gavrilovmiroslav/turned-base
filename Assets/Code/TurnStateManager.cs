using Ilumisoft.VisualStateMachine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TurnStateManager : MonoSingleton<TurnStateManager>
{
    private StateMachine _StateMachine;
    private Queue<Character> GoingNext = new();
    private Queue<Character> AlreadyGone = new();

    public Character Current { get; private set; }

    private void Start()
    {
        _StateMachine = GetComponent<StateMachine>();
    }

    public void TurnState_OnStart()
    {
        StartCoroutine(StartTurnOrder());
    }

    public IEnumerator StartTurnOrder()
    {
        var chars = Character.GetAliveCharacters();
        foreach (var c in chars)
            c.Moves = c.Template.Moves;

        GoingNext = new Queue<Character>(chars);
        AlreadyGone = new Queue<Character>();

        yield return new WaitForSeconds(1.0f);
        _StateMachine.TriggerByLabel("on_pick_next");
    }

    public void EndPlay()
    {
        _StateMachine.TriggerByLabel("on_check_end");
    }

    public void TurnState_OnPickNext()
    { 
        if (GoingNext.Count == 0)
        {
            _StateMachine.TriggerByLabel("on_end");
        }
        else
        {
            StartCoroutine(SelectNext());            
        }
    }

    public IEnumerator SelectNext()
    {
        if (Current != null)
        {
            FlingPhysics.GetInstance().OnHitDetected -= Current.OnHit;
            Current.Visual.GetComponent<SpriteOutline>().directions = SpriteOutline.Directions.OFF;
            AlreadyGone.Enqueue(Current);
        }

        Current = GoingNext.Dequeue();
        CharacterManager.GetInstance().ResetHitStreak();
        FlingPhysics.GetInstance().OnHitDetected += Current.OnHit;
        StartCoroutine(CharacterCard.GetInstance().WaitForViewUpdate(Current));
        yield return CameraPan.GetInstance().PanTo(Current.transform.position);
        yield return new WaitForSeconds(0.2f);
        Current.Visual.GetComponent<SpriteOutline>().directions = SpriteOutline.Directions.ON;

        _StateMachine.TriggerByLabel("on_play");
    }

    public void TurnState_OnPlay()
    {
        Current.GetComponent<AbstractFlingController>()?.Play();
    }

    public void TurnState_OnCheckEnd()
    {
        StartCoroutine(DoAbilities());
    }

    public IEnumerator DoAbilities()
    {
        foreach (var character in Character.GetAllCharacters())
        {
            while (character.AppliedAbilities.Count > 0)
            {
                var ability = character.AppliedAbilities[0];
                character.AppliedAbilities.RemoveAt(0);
                yield return ability.Perform(character);
                character.GetComponentInChildren<CharacterHUD>().UpdateView();
            }
        }

        Current.Moves -= 1;
        CharacterCard.GetInstance().UpdateView(Current, false);
        yield return new WaitForSeconds(1.0f);
        if (Current.Moves > 0)
        {
            
            _StateMachine.TriggerByLabel("on_play");
        }
        else
        {
            _StateMachine.TriggerByLabel("on_pick_next");            
        }
    }

    public void TurnState_OnEnd()
    {
        if (Character.GetAliveCharacters().Count > 0)
        {
            _StateMachine.TriggerByLabel("on_start");
        }
        else
        {
            _StateMachine.TriggerByLabel("on_complete");
        }
    }
}
