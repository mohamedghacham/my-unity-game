using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static bool click = false;
    static bool jumpCool = true;

    public static Trainer trainer = null;
    public static Pokemon pokemon = null;
    public static Item item = null;
    public static bool pokemonActive = false;

    public static GameGUI gamegui;

    void Start()
    {
        trainer = GameObject.Find("Player").GetComponent<Trainer>();
        gameObject.AddComponent<CameraControl>();
        gamegui = GetComponent<GameGUI>();
    }

    void Update()
    {
        if (Dialog.inDialog)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            trainer.SetVelocity(Vector3.zero);
            return;
        }

        if ((GameGUI.menuActive && !pokemonActive) || CameraControl.releaseCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (pokemonActive && pokemon.obj != null)
        {
            trainer.SetVelocity(Vector3.zero);
            Vector3 velocity = Vector3.zero;
            velocity += pokemon.obj.transform.forward * Input.GetAxis("Vertical");
            velocity += pokemon.obj.transform.right * Input.GetAxis("Horizontal");
            velocity *= pokemon.obj.speed;

            pokemon.obj.SetVelocity(velocity);
            pokemon.obj.transform.Rotate(pokemon.obj.transform.up, Input.GetAxis("Mouse X"));

            if (Input.GetButton("Jump") && jumpCool && Physics.Raycast(pokemon.obj.transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f))
            {
                pokemon.obj.GetComponent<Rigidbody>().AddForce(Vector3.up * 3000);
                jumpCool = false;
            }
            if (!Input.GetButton("Jump")) jumpCool = true;

            pokemon.pp -= Time.deltaTime / 500;
            if (pokemon.pp <= 0)
            {
                pokemonActive = false;
                pokemon.obj.Return();
            }
        }
        else
        {
            Vector3 vel = Quaternion.Euler(0, CameraControl.ay, 0) * (Vector3.forward * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal"));
            trainer.SetVelocity(vel);
        }

        if (!click && !pokemonActive)
        {
            Pokemon oldPokemonSelection = pokemon;

            for (int i = 0; i < Mathf.Min(10, trainer.pokemon.Count); i++)
            {
                if (Input.GetKey((KeyCode)((int)KeyCode.Alpha0 + ((i + 1) % 10))) || Input.GetKey((KeyCode)((int)KeyCode.Keypad0 + ((i + 1) % 10))))
                {
                    pokemon = trainer.pokemon[i];
                }
            }

            if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                if (pokemon == trainer.pokemon[0])
                    pokemon = trainer.pokemon[trainer.pokemon.Count - 1];
                else if (trainer.pokemon.Contains(pokemon))
                    pokemon = trainer.pokemon[trainer.pokemon.IndexOf(pokemon) - 1];
            }

            if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
            {
                if (pokemon == trainer.pokemon[trainer.pokemon.Count - 1])
                    pokemon = trainer.pokemon[0];
                else if (trainer.pokemon.Contains(pokemon))
                    pokemon = trainer.pokemon[trainer.pokemon.IndexOf(pokemon) + 1];
            }

            if (oldPokemonSelection != pokemon)
            {
                click = true;
                if (oldPokemonSelection.obj != null)
                {
                    oldPokemonSelection.obj.Return();
                    trainer.ThrowPokemon(pokemon);
                }
            }
        }

        if (!trainer.pokemon.Contains(pokemon)) pokemon = null;
        if (pokemon == null && trainer.pokemon.Count > 0) pokemon = trainer.pokemon[0];

        if (!trainer.inventory.Contains(item)) item = null;
        if (item == null && trainer.inventory.Count > 0) item = trainer.inventory[0];

        if (!click && Input.GetKey(KeyCode.Return))
        {
            if (pokemon.obj == null)
            {
                trainer.ThrowPokemon(pokemon);
            }
            else
            {
                if (pokemonActive)
                {
                    pokemon.obj.Return();
                    pokemonActive = false;
                }
                else
                {
                    pokemonActive = true;
                }
            }
            click = true;
        }

        if (Input.GetKey(KeyCode.Escape) && !click)
        {
            if (pokemonActive)
                pokemonActive = false;
            else
                GameGUI.menuActive = !GameGUI.menuActive;
            click = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CapturePokemon();
            click = true;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            GameGUI.chatActive = !GameGUI.chatActive;
            click = true;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            PokeCenter.HealPokemon();
        }

        if (!Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            click = false;
    }

    public static void CapturePokemon()
    {
        // Future capture logic – update this block when ready to instantiate objects.
        Debug.Log("Capture Pokemon triggered.");
        // Example (must have a prefab "Pokeball" in Resources):
        /*
        Vector3 targetPos = pokemon.obj.transform.position;
        GameObject ball = Instantiate(Resources.Load<GameObject>("Pokeball"));
        Transform throwPoint = GameObject.Find("_PokeballHolder").transform;
        throwPoint.LookAt(targetPos);
        ball.transform.position = throwPoint.position;
        ball.GetComponent<Rigidbody>().AddForce((targetPos - throwPoint.position).normalized * 500 + Vector3.up * 300);
        Pokeball.CapturePokemon();
        Destroy(ball, 2f);
        */
    }
}
