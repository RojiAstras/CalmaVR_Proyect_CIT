using UnityEngine;

public class fadeCamara : MonoBehaviour
{   
    public float timeEvent = 15f;
    //Fade de camara:
    public float speedScale = 0.2f;
    public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 0.5f, -0.5f, -0.5f), new Keyframe(1, 0));
    public bool startFadeOut = true;

    AudioSource musica;
    private bool flagEventMusicaEscenario = false;
    private bool fadeFlag = false; // false: No hace nada; true: Realiza la acción de fade;
    private float alpha = 0f;
    private Texture2D texture;
    private int direction = 0;
    private float time = 0f;
    private GameObject musicaEscenario; 
    

    // Start is called before the first frame update
    void Start()
    {

        musica = GameObject.Find("Ambient_Music_Player").GetComponent<AudioSource>();
        if (startFadeOut) alpha = 1f; else alpha = 0f;
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(Color.black.r, Color.black.g, Color.black.b, alpha));
        texture.Apply(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == 0 && fadeFlag)
        {
            if (alpha >= 1f) // Sin fade en la pantalla
            {
                alpha = 1f;
                time = 0f;
                direction = 1;
                fadeFlag = false;
            }
            else // Fade visible en la pantalla
            {
                alpha = 0f;
                time = 1f;
                direction = -1;
                fadeFlag = false;
            }
        }

        if (timeEvent > 0)
        {
            timeEvent -= Time.deltaTime;

            if (timeEvent <= 0)
            {
                fadeFlag = true; 
                flagEventMusicaEscenario = true;
            } 
        }

        if (flagEventMusicaEscenario)
        {
            musica.Play();
            flagEventMusicaEscenario = false;
        }
    }

    public void OnGUI()
    {
        if (alpha > 0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        if (direction != 0)
        {
            time += direction * Time.deltaTime * speedScale;
            alpha = Curve.Evaluate(time);
            texture.SetPixel(0, 0, new Color(Color.black.r, Color.black.g, Color.black.b, alpha));
            texture.Apply();
            if (alpha <= 0f || alpha >= 1f) direction = 0; 
        }
    }
}
