using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.UI;


public class UnitController : NetworkBehaviour, IUnit
{
    private UnitData data;
    public UnitData Data => data;
    [SerializeField] private SpriteRenderer selectedCircle;
    private int teamID;

    public int TeamId => teamID;

    private UnitAI AI;
    private Mover mover;
    private Sight sight;
    private SpriteRenderer visual;
    private Animator animator;
    private bool[] AbilitiesAvalable;
    private int currentAbility;
    private bool castingLock;


    public void Init(UnitData dataPrefab, int teamID, Transform parent)
    {
        transform.parent = parent;
        this.teamID = teamID;
        this.data = Instantiate(dataPrefab);
        this.data.Init();
        mover = GetComponent<Mover>();

        sight = GetComponentInChildren<Sight>();
        visual = GetComponentInChildren<SpriteRenderer>();
        animator = visual.GetComponent<Animator>();
        mover.Init(transform, visual.transform);
        AI = Instantiate(data.AI);
        AI.Init(mover, sight, animator, data);
        animator.runtimeAnimatorController = data.AnimsController;
        selectedCircle.transform.localScale = (data.size.Value / 32) * .5f * Vector2.one;
        AbilitiesAvalable = new bool[data.Abilities.Count];
        for (int i = 0; i < AbilitiesAvalable.Length; i++)
        {
            AbilitiesAvalable[i] = true;
        }

        this.name = data.Name;
        this.GetComponent<Health>().Init(data);
        this.GetComponent<Selectable>().Init();
        this.GetComponent<Rigidbody2D>().mass = data.mass.Value;
        this.GetComponentInChildren<SpriteRenderer>().sprite = data.Sprite;
        this.GetComponent<CircleCollider2D>().radius = data.size.Value / 200;
    }

    private void FixedUpdate()
    {
        if (AI != null)
            AI.DoSomeThing();
    }

    public bool OverrideCommand(Command command)
    {
        return AI.Execute(command);
    }

    public void SetSelected(bool isSelected)
    {
        selectedCircle.enabled = isSelected;
    }

    public Sprite GetSprite()
    {
        return visual.sprite;
    }

    public int GetNumber()
    {
        return 1;
    }

    public bool Contains(IUnit unit)
    {
        if (unit == (IUnit) this)
        {
            return true;
        }

        return false;
    }

    public bool AddCommand(Command command)
    {
        return AI.AddCommand(command);
    }

    public void OnCastFinish()
    {
        castingLock = false;
    }

    public bool CastComplexeAbility(int Index, Transform target)
    {
        if (AbilitiesAvalable[Index])
        {
            StartCoroutine(StartComplexeAbilityRoutine(Index, target));
        }

        return true;
    }

    public bool CastBasicAbility(int Index, Vector2 position)
    {
        if (AbilitiesAvalable[Index])
        {
            currentAbility = Index;
            StartCoroutine(StartBasicAbilityRoutine(Index, position));
        }

        return true;
    }

    private IEnumerator StartBasicAbilityRoutine(int Index, Vector2 positon)
    {
        AbilitiesAvalable[Index] = false;
        animator.SetTrigger("Ability" + Index);
        castingLock = data.Abilities[Index].CastLock;
        while (castingLock)
        {
            yield return null;
        }

        data.Abilities[Index].Cast(positon);
        yield return new WaitForSeconds(data.Abilities[Index].Cooldown);

        AbilitiesAvalable[Index] = true;
    }

    private IEnumerator StartComplexeAbilityRoutine(int Index, Transform target)
    {
        AbilitiesAvalable[Index] = false;
        animator.SetTrigger("Ability" + Index);
        data.Abilities[Index].Cast(target);
        castingLock = data.Abilities[Index].CastLock;
        while (castingLock)
        {
            yield return null;
        }

        yield return new WaitForSeconds(data.Abilities[Index].Cooldown);

        AbilitiesAvalable[Index] = true;
    }
}