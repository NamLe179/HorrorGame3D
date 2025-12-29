using System;
using System.Collections;
using UnityEngine;

namespace HorrorGame3D.Interaction
{
    public class SafeLockMiniGame : MonoBehaviour
    {
        

        [Header("UI")]
        [SerializeField] private SafeLockUI ui;

        [Header("References")]
        [SerializeField] private Transform dial; // object thật trong scene
        [SerializeField] private Transform focusPoint; // gán FocusPoint (position+rotation)
        [SerializeField] private Camera safeCamera;

        [Header("Audio")]
        [SerializeField] private AudioSource rotateSound;
        [SerializeField] private AudioSource clickSound;

        [Header("Lock Settings")]
        public int[] combination = { 42, 17, 88 };
        public float correctThreshold = 0.1f;
        public float unlockDelay = 1f;

        private int step = 0;
        private float lastInput = 0f;  // -1 = A, +1 = D
        private float lastNumber = 0f;
        private float currentNumber = 0f;
        private bool isActive = false;

        public event System.Action OnUnlock;

        void Start()
        {
            if (dial == null) Debug.LogWarning("SafeLockMiniGame: dial not assigned!");
            if (focusPoint == null) Debug.LogWarning("SafeLockMiniGame: focusPoint not assigned!");
        }

        void Update()
        {
            //if (!isActive) return;

            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    ExitMiniGame();
            //    return;
            //}

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    dial.Rotate(Vector3.forward, -3.6f);

            //    currentNumber = GetDialValue();
            //    playSound();

            //    if (lastInput == 1 && step < combination.Length)
            //    {
            //        if (CheckNumberCorrect(lastNumber))
            //        {
            //            if (ui) ui.UpdateStep(step, combination[step]);
            //            step++;

            //            if (step >= combination.Length)
            //            {
            //                StartCoroutine(UnlockDelay());
            //            }
            //        }
            //        else
            //        {
            //            ExitMiniGame();
            //            return;
            //        }
            //    }

            //    lastInput = -1;
            //    lastNumber = currentNumber;
            //}
            //else if (Input.GetKeyDown(KeyCode.D))
            //{
            //    dial.Rotate(Vector3.forward, 3.6f);

            //    currentNumber = GetDialValue();
            //    playSound();

            //    if (lastInput == -1 && step < combination.Length)
            //    {
            //        if (CheckNumberCorrect(lastNumber))
            //        {
            //            if (ui) ui.UpdateStep(step, combination[step]);
            //            step++;

            //            if (step >= combination.Length)
            //            {
            //                StartCoroutine(UnlockDelay());
            //            }
            //        }
            //        else
            //        {
            //            ExitMiniGame();
            //            return;
            //        }
            //    }

            //    lastInput = 1;
            //    lastNumber = currentNumber;

            //}
            //else return;

            if (!isActive) return;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ExitMiniGame();
                return;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                dial.Rotate(Vector3.forward, -3.6f);

                currentNumber = GetDialValue();
                playSound();

                if (lastInput == 1 && step < combination.Length)
                {
                    if (CheckNumberCorrect(lastNumber) <= correctThreshold)
                    {
                        if (ui) ui.UpdateStep(step, combination[step]);
                        step++;

                        if (step >= combination.Length)
                        {
                            StartCoroutine(UnlockDelay());
                        }
                    }
                    else
                    {
                        ExitMiniGame();
                        return;
                    }
                }

                lastInput = -1;
                lastNumber = currentNumber;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                dial.Rotate(Vector3.forward, 3.6f);

                currentNumber = GetDialValue();
                playSound();

                if (lastInput == -1 && step < combination.Length)
                {
                    if (CheckNumberCorrect(lastNumber) <= correctThreshold)
                    {
                        if (ui) ui.UpdateStep(step, combination[step]);
                        step++;

                        if (step >= combination.Length)
                        {
                            StartCoroutine(UnlockDelay());
                        }
                    }
                    else
                    {
                        ExitMiniGame();
                        return;
                    }
                }

                lastInput = 1;
                lastNumber = currentNumber;

            }
            else return;
        }

        //public void playSound()
        //{
        //    if (currentNumber != lastNumber && CheckNumberCorrect(currentNumber) && clickSound)
        //    {
        //        clickSound.Play();
        //    }
        //    else
        //    {
        //        if (rotateSound && !rotateSound.isPlaying) rotateSound.Play();
        //    }
        //}


        public void playSound()
        {
            float diff = CheckNumberCorrect(currentNumber);

            if
                (currentNumber != lastNumber
                && diff <= correctThreshold
                && clickSound)
            {
                clickSound.Play();
            }
            else
            {
                if (rotateSound)
                {
                    float volume = 0.75f;

                    if (diff <= 10f)
                    {
                        volume += diff / 25f;
                    }

                    rotateSound.PlayOneShot(rotateSound.clip, volume);
                }

            }
        }
        float GetDialValue()
        {
            float z = Mathf.Repeat(dial.localEulerAngles.z, 360f);
            z = 100f - (z / 3.6f);

            if (z % 10 <= 0.1 || z % 10 >= 0.9) return Mathf.Round(z);
            else return z;
        }

        float CheckNumberCorrect(float value)
        {
            float target = combination[step];
            return Mathf.Abs(value - target);
        }

        public void StartMiniGame()
        {
            PauseGameInput.OnLoadMinigame?.Invoke();
            if (safeCamera) safeCamera.enabled = true;
            PlayerMoveAndCameraController.DisableAll();
            if (ui) ui.ShowUI();
            isActive = true;
            step = 0;
        }

        private IEnumerator UnlockDelay()
        {
            yield return new WaitForSeconds(unlockDelay);
            if(OnUnlock!= null) OnUnlock();
            else Debug.Log("OnUnlock event not assigned!");
                Debug.Log("Safe unlocked!");
            ExitMiniGame();
        }

        private void ExitMiniGame()
        {
            PauseGameInput.OnUnloadMinigame?.Invoke();
            step = 0;
            lastInput = 0f;
            lastNumber = 0f;
            currentNumber = 0f;
            dial.localRotation = Quaternion.Euler(0f, 0f, 0f);
            if (safeCamera) safeCamera.enabled = false;
            PlayerMoveAndCameraController.EnableAll();
            if (ui) ui.HideUI();
            isActive = false;
        }

    }
}