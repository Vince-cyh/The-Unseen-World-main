using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] RPGUI rpgui;
    [SerializeField] int Heal;
    [SerializeField] int Injure;

    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    public LayerMask grassLayer2;
    public LayerMask grassLayer3;
    public LayerMask grassLayer4;
    public LayerMask fruitLayer;
    public LayerMask InjureLayer;
    public LayerMask interactableLayer;
    Rigidbody2D rigidbody2D;
    Vector2 lookDirection = new Vector2(1, 0);

    public event Action<Collider2D> OnEncountered;

    private bool isMoving;
    private Vector2 input;

    public static Animator animator;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (LightManager.CheckForWin())
        {
            SceneManager.LoadScene(5);
        }
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // 防止对角线移动
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);

        // 按Z互动
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();

        RaycastHit2D hit = Physics2D.Raycast(rigidbody2D.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
            if (character != null)
            {
                character.DisplayDialog();
            }
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.1f, interactableLayer);
        if (collider != null)
        {
            string objectName = collider.gameObject.name;

            if (objectName == "Victory")
            {
                collider.GetComponent<Interactable>()?.Cheater(collider);
                // 这里执行 GameObject1 特有的交互操作
            }
            else
            {
                collider.GetComponent<Interactable>()?.Interact(collider);
                // 这里执行 GameObject2 特有的交互操作
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
        CheckForFruit();
        CheckForInjure();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if ((Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer) != null) |
           ( Physics2D.OverlapCircle(targetPos, 0.1f, interactableLayer) != null))
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer);
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered(collider);
            }
        }
        else if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer2) != null)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer2);
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered(collider);
            }
        }
        else if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer3) != null)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer3);
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered(collider);
            }
        }
        else if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer4) != null)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer4);
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered(collider);
            }
        }
    }

    private void CheckForFruit()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, fruitLayer) != null)
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, fruitLayer);
            if (rpgui.Healing(Heal))
            {
                Destroy(collider.gameObject);
                SoundManager.PlayPickUp();
            }
        }
    }

    private void CheckForInjure()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, InjureLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                rpgui.Injuring(Injure);
            }
        }
    }
}