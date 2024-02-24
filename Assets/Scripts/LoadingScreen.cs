using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public float delay = 3f; // D�lai en secondes avant de charger la sc�ne
    public string sceneToLoad = "Menu"; // Nom de la sc�ne � charger
    public AudioClip[] loadingSounds; // Tableau pour stocker les diff�rents AudioClips

    private AudioSource audioSource; // AudioSource pour jouer le son
    

    private void Start()
    {
        // Initialiser l'AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        

        // Choisir un AudioClip al�atoire et le jouer
        if (loadingSounds.Length > 0)
        {
            audioSource.clip = loadingSounds[Random.Range(0, loadingSounds.Length)];
            audioSource.Play();
        }

        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToLoad);
    }
}
