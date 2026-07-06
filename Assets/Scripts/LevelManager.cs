using UnityEngine;
using UnityEngine.SceneManagement;

// Diğer tüm Awake'lerden önce çalışması lazım
[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] OrderData[] levels;

    int _currentIndex;

    public int       CurrentIndex => _currentIndex;
    public int       TotalLevels  => levels?.Length ?? 0;
    public bool      IsLastLevel  => _currentIndex >= (levels?.Length ?? 0) - 1;
    public OrderData CurrentLevel =>
        levels != null && _currentIndex < levels.Length ? levels[_currentIndex] : null;

    void Awake()
    {
        // Sahne yenilenince eski instance'ı koru, yenisini yok et
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NextLevel()
    {
        _currentIndex = IsLastLevel ? 0 : _currentIndex + 1;
        Reload();
    }

    public void RestartLevel() => Reload();

    static void Reload() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
