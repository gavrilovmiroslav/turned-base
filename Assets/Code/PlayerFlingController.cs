using System.Collections;

using UnityEngine;

public class PlayerFlingController
    : AbstractFlingController
{
    public bool FlingInProgress = false;
    public float Timer = 0;

    private bool _Thinking = false;
    private Vector3 _InputPosition;
    private Collider2D _Collider;
    private Ray2D _FlingingRay;
    private Ray2D _AimingRay;
    private float _AimingDistance;

    public void Start()
    {
        _Collider = GetComponent<Collider2D>();
    }

    public override void Play()
    {
        _Thinking = true;
    }

    public void Update()
    {
        if (!_Thinking) return;

        if (TurnStateManager.GetInstance().Current != Character) return;

        if (Character.Health <= 0)
            return;

        Timer += Time.deltaTime;
        if (Timer >= 1.0f)
            Timer = 0.0f;

        _InputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _InputPosition.z = 0.0f;

        var indicators = SignFeedbackManager.GetInstance().InstantiatedFlingIndicatorDots;
        var count = SignFeedbackManager.GetInstance().FlingIndicatorDotsCount;

        if (!FlingInProgress)
        {
            for (int i = 0; i < count; i++)
            {
                indicators[i].SetActive(false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                CameraPan.CanPan = false;
                if (_Collider.OverlapPoint(_InputPosition))
                {
                    FlingInProgress = true;
                }
            }
            else
            {
                CameraPan.CanPan = true;
            }
        }
        else
        {
            var pos = this.transform.position;
            pos.z = 0.0f;

            _FlingingRay.origin = pos;
            _FlingingRay.direction = _InputPosition - pos;

            _AimingRay.origin = pos;
            _AimingRay.direction = pos - _InputPosition;

            _AimingDistance = Vector3.Distance(pos, _InputPosition);

            var min = 0.2f;
            var max = 4.0f;

            var target = _InputPosition;
            if (_AimingDistance > max)
            {
                target = _FlingingRay.GetPoint(max);
            }

            var color = Color.white;
            if (_AimingDistance > min)
            {
                var pts = Mathf.Min(count - 1, Mathf.RoundToInt(_AimingDistance * 10.0f));

                for (int i = 0; i < count; i++)
                {
                    var visible = i < pts;
                    indicators[i].SetActive(visible);
                    if (visible)
                    {
                        var t = (i + Timer) * (1.0f / count);
                        indicators[i].transform.position = _AimingRay.GetPoint(min + t);
                        color.a = 1 - t;
                        indicators[i].GetComponent<SpriteRenderer>().color = color;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    CameraPan.CanPan = true;
                    FlingInProgress = false;

                    _AimingDistance = Mathf.Round(_AimingDistance * 10.0f) / 10.0f;
                    if (_AimingDistance > 1.0f) _AimingDistance = 1.0f;

                    var speed = _AimingDistance * 2.0f * _AimingRay.direction;

                    for (int i = 0; i < count; i++)
                    {
                        indicators[i].SetActive(false);
                    }

                    StartCoroutine(PlayMove(speed));
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    indicators[i].SetActive(false);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    FlingInProgress = false;
                    CameraPan.CanPan = true;
                }
            }
        }
    }

    public IEnumerator PlayMove(Vector2 speed)
    {
        yield return new WaitForSeconds(0.25f);
        _Thinking = false;

        var body = GetComponent<FlingBody>();
        if (body != null)
        {
            body.SetSpeed(speed * (10.0f / (float)Character.Configuration.Weight));
        }

        yield return CharacterCard.GetInstance().WaitForOffscreen();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FlingPhysics.GetInstance().WaitForFlingsToEnd());
        GetComponentInChildren<SpriteOutline>().OutlineColor = Color.gray;
        yield return new WaitForSeconds(1.0f);
        Done();
    }
}