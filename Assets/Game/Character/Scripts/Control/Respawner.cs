using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        Health playerHealth;
        [SerializeField] Transform respawnLocation = null;
        [SerializeField] float fadeTime = 0.5f;

        private void Awake() 
        {
            playerHealth = GetComponent<Health>();
        }

        private void Start() 
        {
            playerHealth.OnDie.AddListener(Respawn);
            if(!playerHealth.IsAlive()) Respawn();
        }

        void Respawn()
        {
            StartCoroutine(RespawnCorroutine());
        }

        IEnumerator RespawnCorroutine()
        {
            FindObjectOfType<SavingWrapper>().Save();
            yield return new WaitForSeconds(fadeTime);
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeTime);
            RespawnPlayer();
            FindObjectOfType<SavingWrapper>().Save();
            yield return fader.FadeIn(fadeTime);
        }

        void RespawnPlayer()
        {
            Vector3 delta = respawnLocation.position - transform.position;
            GetComponent<NavMeshAgent>().Warp(respawnLocation.position);
            playerHealth.Respawn();
            ResetEnemies();
            ICinemachineCamera activeCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            if(activeCamera.Follow == transform)
            {
                activeCamera.OnTargetObjectWarped(transform, delta);
            }
        }

        void ResetEnemies()
        {
            foreach(AIController enemy in FindObjectsOfType<AIController>())
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if(!enemyHealth) continue;
                if(!enemyHealth.IsAlive()) continue;
                enemy.ResetEnemy();
                enemyHealth.Heal((int)(enemyHealth.GetMaxHealth() * 0.5f));
            }
        }
    }
}