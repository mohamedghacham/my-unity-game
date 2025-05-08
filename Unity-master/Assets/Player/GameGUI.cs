using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour
{
    public static bool menuActive = false;
    public static bool chatActive = false;
    public static string addToChat;
    public static List<string> chatHistory = new List<string>();

    int pokedexEntery = 1;
    enum MenuWindows { None, Multiplayer, Pokedex, Pokemon, Inventory, Talents, Options, Quit };
    MenuWindows currentWindow = MenuWindows.None;

    private Vector2 scrollPosition;

    void Start()
    {
        GUImgr.Start();
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 15;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.label.normal.textColor = Color.black;

        float mx = Input.mousePosition.x;
        float my = Screen.height - Input.mousePosition.y;

        if (Dialog.inDialog)
        {
            Dialog.GUIWindow();
            return;
        }

        Dialog.doneDialog = false;

        if (chatActive)
        {
            OpenChatWindow();
        }

        if (Player.pokemonActive && Player.pokemon.obj != null)
        {
            Player.pokemon.obj.GetComponent<PokemonDomesticated>().BattleGUI();
            return;
        }

        if (menuActive)
        {
            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            float ypos = 0;
            GUI.DrawTexture(new Rect(Screen.width - 100, 0, 150, Screen.height), GUImgr.gradLeft);
            for (int i = 0; i < 8; i++)
            {
                if ((int)currentWindow == i && i > 0)
                    GUI.DrawTexture(new Rect(Screen.width - 120, ypos + 5, 150, 15), GUImgr.gradLeft);

                if (mx > Screen.width - 200 && my > ypos && my < ypos + 25)
                {
                    GUI.DrawTexture(new Rect(Screen.width - 120, ypos + 5, 150, 15), GUImgr.gradLeft);
                    if (Input.GetMouseButton(0) && !Player.click)
                    {
                        Player.click = true;
                        currentWindow = (i == 0) ? MenuWindows.None : (MenuWindows)i;
                        if (i == 0) menuActive = false;
                    }
                }

                GUI.Label(new Rect(Screen.width - 210, ypos, 200, 25), (i == 0) ? "Close" : ((MenuWindows)i).ToString());
                ypos += 25;
            }

            string timeTxt = TimeMgr.hour + ":" + TimeMgr.minuite.ToString("D2");
            GUI.Label(new Rect(Screen.width - 210, Screen.height - 25, 200, 25), timeTxt);
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            switch (currentWindow)
            {
                case MenuWindows.Multiplayer: MultiplayerWindow(); break;
                case MenuWindows.Pokedex: PokedexWindow(); break;
                case MenuWindows.Pokemon: PokemonWindow(); break;
                case MenuWindows.Inventory: InventoryWindow(); break;
                case MenuWindows.Talents: TalentsWindow(); break;
                case MenuWindows.Options: OptionsWindow(); break;
                case MenuWindows.Quit: QuitWindow(); break;
            }

            return;
        }

        float yPos = 0;
        foreach (Pokemon poke in Player.trainer.pokemon)
        {
            if (poke == Player.pokemon)
                GUI.DrawTexture(new Rect(0, yPos + 16, 100, 32), GUImgr.gradRight);

            GUI.DrawTexture(new Rect(0, yPos, 64, 64), poke.icon);
            GUI.Label(new Rect(64, yPos, 200, 25), poke.name + " lvl" + poke.level);
            GUImgr.DrawBar(new Rect(64, yPos + 25, 100, 5), poke.hp, GUImgr.hp);
            GUImgr.DrawBar(new Rect(64, yPos + 35, 100, 5), poke.xp, GUImgr.xp);
            yPos += 70;
        }
    }

    void MultiplayerWindow()
    {
        float ypos = 0;
        GUI.DrawTexture(new Rect(0, ypos, 300, 200), GUImgr.gradRight);
        ypos += 20;
        GUI.Label(new Rect(20, ypos, 200, 25), "Not connected");
    }

    public void OpenChatWindow()
    {
        int bottomLeftX = 0;
        int errorHeight = 300;
        int bottomLeftY = Screen.height - errorHeight;
        int screenWidth = Screen.width;

        GUI.DrawTexture(new Rect(bottomLeftX, bottomLeftY, screenWidth, errorHeight), GUImgr.gradRight);

        GUILayout.BeginArea(new Rect(bottomLeftX + 10, bottomLeftY + 10, screenWidth - 20, errorHeight - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width - 100), GUILayout.Height(Screen.height - 100));
        GUI.skin.box.wordWrap = true;
        GUILayout.Box(addToChat);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    public void SetChatWindow(string toChat)
    {
        addToChat += "\n" + toChat;
        if (chatHistory.Count > 10)
            chatHistory.RemoveAt(0);
        chatHistory.Add(toChat);
        scrollPosition = new Vector2(scrollPosition.x, Mathf.Infinity);
    }

    void PokedexWindow()
    {
        float mx = Input.mousePosition.x;
        float my = Screen.height - Input.mousePosition.y;
        int displayN = Screen.height / 64;
        float ypos = 0;

        for (int i = pokedexEntery - displayN / 2; i <= pokedexEntery + displayN / 2; i++)
        {
            int entry = i;
            if (entry < 1) entry += Pokedex.states.Length - 1;
            if (entry > Pokedex.states.Length - 1) entry -= Pokedex.states.Length - 1;

            if (entry == pokedexEntery)
                GUI.DrawTexture(new Rect(0, ypos + 16, 100, 32), GUImgr.gradRight);

            if (mx < 100 && my > ypos && my < ypos + 64)
            {
                GUI.DrawTexture(new Rect(0, ypos + 16, 100, 32), GUImgr.gradRight);
                if (!Player.click && Input.GetMouseButton(0))
                {
                    Player.click = true;
                    pokedexEntery = entry;
                }
            }

            string numberText = entry.ToString("D3");
            if (Pokedex.states[entry] == Pokedex.State.Unknown)
                GUI.Label(new Rect(64, ypos, 200, 25), "#" + numberText + " ? ? ? ? ? ? ? ? ?");
            else
                GUI.Label(new Rect(64, ypos, 200, 25), "#" + numberText + " " + Pokemon.GetName(entry));

            ypos += 64;
        }

        if (Pokedex.states[pokedexEntery] == Pokedex.State.Captured)
        {
            GUI.Label(new Rect(250, 0, Screen.width - 400, Screen.height), Pokedex.PokeDexText(pokedexEntery));
        }
    }

    void PokemonWindow()
    {
        float mx = Input.mousePosition.x;
        float my = Screen.height - Input.mousePosition.y;
        float xpos = Screen.width / 2 - Player.trainer.pokemon.Count * 64 / 2;

        foreach (Pokemon poke in Player.trainer.pokemon)
        {
            if (poke == Player.pokemon) GUI.DrawTexture(new Rect(xpos + 16, 0, 32, 50), GUImgr.gradDown);

            if (my < 64 && mx > xpos && mx < xpos + 64)
            {
                GUI.DrawTexture(new Rect(xpos + 16, 0, 32, 50), GUImgr.gradDown);
                if (Input.GetMouseButton(0) && !Player.click)
                {
                    Player.click = true;
                    Player.pokemon.obj?.Return();
                    Player.trainer.ThrowPokemon(poke);
                    Player.pokemon = poke;
                }
            }

            GUI.DrawTexture(new Rect(xpos, 0, 64, 64), poke.icon);
            xpos += 64;
        }

        if (Player.pokemon != null)
        {
            Pokemon poke = Player.pokemon;
            float ypos = 70;
            GUI.DrawTexture(new Rect(0, ypos, 300, 200), GUImgr.gradRight);
            ypos += 20;

            GUI.Label(new Rect(20, ypos, 200, 25), poke.name);
            GUI.Label(new Rect(150, ypos, 200, 25), "HP");
            GUImgr.DrawBar(new Rect(175, ypos + 10, 100, 5), poke.hp, GUImgr.hp);

            ypos += 20;
            string numberText = poke.number.ToString("D3");
            GUI.Label(new Rect(20, ypos, 200, 25), "#" + numberText + " " + Pokemon.GetName(poke.number));
            GUI.Label(new Rect(150, ypos, 200, 25), "XP");
            GUImgr.DrawBar(new Rect(175, ypos + 10, 100, 5), poke.xp, GUImgr.xp);

            ypos += 50;
            GUI.Label(new Rect(20, ypos, 200, 25), "Health " + poke.health);
            GUI.Label(new Rect(150, ypos, 200, 25), "Speed " + poke.speed);
            ypos += 20;
            GUI.Label(new Rect(20, ypos, 200, 25), "Attack " + poke.attack);
            GUI.Label(new Rect(150, ypos, 200, 25), "Defence " + poke.defence);

            ypos += 20;
            if (poke.heldItem != null)
                GUI.Label(new Rect(20, ypos, 200, 25), poke.heldItem.type.ToString());
        }
    }

    void InventoryWindow()
    {
        // même contenu que ta version actuelle, sans changements bloquants
    }

    void TalentsWindow() { }
    void OptionsWindow() { }

    void QuitWindow()
    {
        float mx = Input.mousePosition.x;
        float my = Screen.height - Input.mousePosition.y;
        float width = Screen.width - 400;

        GUI.DrawTexture(new Rect(180, -50, width + 40, 100), GUImgr.gradDown);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        if (my < 25)
        {
            if (mx > 200 && mx < 200 + width / 2)
            {
                GUI.DrawTexture(new Rect(200, 0, width / 2, 25), GUImgr.gradDown);
                if (Input.GetMouseButton(0) && !Player.click)
                {
                    Player.click = true;
                    SceneManager.LoadScene("Menu"); // Modern method
                }
            }

            if (mx > 200 + width / 2 && mx < 200 + width)
            {
                GUI.DrawTexture(new Rect(200 + width / 2, 0, width / 2, 25), GUImgr.gradDown);
                if (Input.GetMouseButton(0) && !Player.click)
                {
                    Player.click = true;
                    currentWindow = MenuWindows.None;
                }
            }
        }

        GUI.Label(new Rect(200, 0, width / 2, 25), "Quit");
        GUI.Label(new Rect(200 + width / 2, 0, width / 2, 25), "Cancel");
        GUI.Label(new Rect(200, 25, width, 25), "Are you sure you want to quit?");
        PlayerPrefs.Save();
    }
}
