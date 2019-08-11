using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameOptionPanel[] GameOptions = null;

    private int _currentOption = 0;

    private float _moveCoolDown = 0.12f;
    private float _moveTimeLeft = 0f;

    private void Update()
    {
        if (_moveTimeLeft > 0)
            _moveTimeLeft -= Time.deltaTime;

        if (_moveTimeLeft <= 0)
        {
            switch (Input.GetAxisRaw("Vertical"))
            {
                case 0:
                    return;

                case 1:
                    if (_currentOption == 0)
                        return;

                    _currentOption--;
                    break;

                case -1:
                    if (_currentOption == GameOptions.Length - 1)
                        return;

                    _currentOption++;
                    break;
            }

            SetSelection();
        }
    }

    private void SetSelection()
    {
        _moveTimeLeft = _moveCoolDown;

        for (int i = 0; i < GameOptions.Length; i++)
        {
            GameOptions[i].SetIsSelected(i == _currentOption);
        }
    }
}
