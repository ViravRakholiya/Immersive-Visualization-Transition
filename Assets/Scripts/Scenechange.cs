using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenechange : MonoBehaviour
{
    public void MoveToScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}
