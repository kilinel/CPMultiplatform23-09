using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
public class PokemonARController : MonoBehaviour
{
    [Header("Pokemon")]
    [SerializeField] string pokemonName = "pikachu";

    [Header("Interacao")]
    [SerializeField] float rotationSpeed = 0.3f;
    [SerializeField] AudioSource pokemonSound;

    [Header("Efeito")]
    [SerializeField] ParticleSystem spawnParticles;

    [Header("Textos iniciais")]
    [SerializeField] TMP_Text foundText;
    [SerializeField] TMP_Text interactionText;

    [Header("Pokedex")]
    [SerializeField] GameObject pokedexPanel;

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text numberText;
    [SerializeField] TMP_Text typeText;
    [SerializeField] TMP_Text heightText;
    [SerializeField] TMP_Text weightText;
    [SerializeField] TMP_Text abilityText;

    [SerializeField] RawImage pokemonImage;

    Vector2 startPosition;
    bool dragging;

    void OnEnable()
    {
        // ativa pokemon
        if (spawnParticles != null)
            spawnParticles.Play();

        if (foundText != null)
            foundText.text = "Pokemon encontrado!";

        if (interactionText != null)
            interactionText.text =
                "Arraste para girar • Toque para taunt";

        StartCoroutine(LoadPokemon());
    }

    void Update()
    {
        HandleTouch();
        HandleMouse();
    }


    void HandleTouch()
    {
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startPosition = touch.position;
            dragging = false;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            if (Vector2.Distance(
                startPosition,
                touch.position) > 10f)
            {
                dragging = true;
            }

            if (dragging)
            {
                transform.Rotate(
                    0,
                    -touch.deltaPosition.x * rotationSpeed,
                    0
                );
            }
        }

        if (touch.phase == TouchPhase.Ended && !dragging)
        {
            CheckClick(touch.position);
        }
    }


    void HandleMouse()
    {
        if (Input.touchCount > 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            dragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 current = Input.mousePosition;

            if (Vector2.Distance(
                startPosition,
                current) > 10f)
            {
                dragging = true;
            }

            if (dragging)
            {
                float x = Input.GetAxis("Mouse X");

                transform.Rotate(
                    0,
                    -x * rotationSpeed * 30,
                    0
                );
            }
        }

        if (Input.GetMouseButtonUp(0) && !dragging)
        {
            CheckClick(Input.mousePosition);
        }
    }

    void CheckClick(Vector2 screenPosition)
    {
        Ray ray =
            Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
            {
                Taunt();
            }
        }
    }

    void Taunt()
    {
        if (pokemonSound != null)
            pokemonSound.Play();

        OpenPokedex();
    }

    public void OpenPokedex()
    {
        if (pokedexPanel != null)
            pokedexPanel.SetActive(true);
    }

    public void ClosePokedex()
    {
        if (pokedexPanel != null)
            pokedexPanel.SetActive(false);
    }

    IEnumerator LoadPokemon()
    {
        string url =
            "https://pokeapi.co/api/v2/pokemon/"
            + pokemonName.ToLower();

        using UnityWebRequest request =
            UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result !=
            UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "Erro PokeAPI: " + request.error);

            yield break;
        }

        PokemonData pokemon =
            JsonUtility.FromJson<PokemonData>(
                request.downloadHandler.text
            );

        UpdatePokedex(pokemon);
    }

    void UpdatePokedex(PokemonData pokemon)
    {
        nameText.text =
            FirstLetterUpper(pokemon.name);

        numberText.text =
            "#" + pokemon.id.ToString("000");

        heightText.text =
            "Altura: " +
            (pokemon.height / 10f) + " m";

        weightText.text =
            "Peso: " +
            (pokemon.weight / 10f) + " kg";


        string types = "";

        foreach (PokemonType type in pokemon.types)
        {
            if (types != "")
                types += " / ";

            types += FirstLetterUpper(
                type.type.name
            );
        }

        typeText.text = "Tipo: " + types;



        string abilities = "";

        foreach (
            PokemonAbility ability
            in pokemon.abilities)
        {
            if (abilities != "")
                abilities += ", ";

            abilities += FirstLetterUpper(
                ability.ability.name
            );
        }

        abilityText.text =
            "Habilidades: " + abilities;

        if (pokemon.sprites != null &&
    !string.IsNullOrEmpty(pokemon.sprites.front_default))
        {
            StartCoroutine(
                LoadPokemonImage(
                    pokemon.sprites.front_default
                )
            );
        }
    }

    IEnumerator LoadPokemonImage(string url)
    {
        using UnityWebRequest request =
            UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result ==
            UnityWebRequest.Result.Success)
        {
            Texture2D texture =
                DownloadHandlerTexture.GetContent(request);

            pokemonImage.texture = texture;
        }
        else
        {
            Debug.LogError(
                "Erro ao baixar imagem: "
                + request.error
            );
        }
    }

    string FirstLetterUpper(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return
            char.ToUpper(text[0])
            + text.Substring(1);
    }
}



[System.Serializable]
public class PokemonData
{
    public int id;
    public string name;
    public int height;
    public int weight;

    public PokemonType[] types;
    public PokemonAbility[] abilities;

    public PokemonSprites sprites;
}

[System.Serializable]
public class PokemonType
{
    public PokemonTypeName type;
}

[System.Serializable]
public class PokemonTypeName
{
    public string name;
}

[System.Serializable]
public class PokemonAbility
{
    public PokemonAbilityName ability;
}

[System.Serializable]
public class PokemonAbilityName
{
    public string name;
}

[System.Serializable]
public class PokemonSprites
{
    public string front_default;
}