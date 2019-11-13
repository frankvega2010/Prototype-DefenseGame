﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretSpawner : MonoBehaviourSingleton<TurretSpawner>
{
    public delegate void OnSpawnerAction();
    public OnSpawnerAction OnSpawnerSpawnTurret;
    public OnSpawnerAction OnSpawnerDeleteTurret;

    [Header("General Settings")]
    public KeyCode activateKey;
    public LayerMask Mask;
    public LayerMask deleteTurretMask;
    public float fireRate;

    [Header("Assign Components/GameObjects")]
    public Button turretButton;
    public GameObject turretTemplate;
    public Shader spawnShader;

    [Header("Checking Variables")]
    public List<GameObject> spawnedTurrets;
    public bool preview;
    public bool canDelete;

    private UITowersState towerUIState;
    private Turret turretProperties;
    private MeshRenderer turretMaterial;
    private GameObject myEventSystem;
    private GameObject newTurretPreview;
    private MaterialPropertyBlock material;
    private bool canSpawn;

    // Start is called before the first frame update
    private void Start()
    {
        myEventSystem = GameObject.Find("EventSystem");

        newTurretPreview = Instantiate(turretTemplate, turretTemplate.transform.position, turretTemplate.transform.rotation);
        turretProperties = newTurretPreview.GetComponent<Turret>();
        turretMaterial = newTurretPreview.transform.GetChild(2).transform.GetChild(0).GetComponent<MeshRenderer>();
        turretProperties.isPreview = true;
        newTurretPreview.GetComponent<BoxCollider>().isTrigger = true;
        turretProperties.turretRadius.gameObject.GetComponent<BoxCollider>().enabled = false;
        newTurretPreview.SetActive(false);

        turretTemplate.GetComponent<FauxGravityBody>().isBuilding = true;

        material = new MaterialPropertyBlock();
        canSpawn = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(activateKey))
        {
            SwitchPreview();
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (canDelete)
            {
                DeleteTurret();
            }
        }        
        
        if(preview)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (turretProperties.canBePlaced && turretProperties.isInTurretZone)
                {
                    if(canSpawn)
                    {
                        Spawn();
                    }
                }
            }

            PreviewTurret();
        }
    }

    private void Spawn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 999, Mask))
        {
            if (hit.transform.gameObject.tag != "turret")
            {
                if(spawnedTurrets.Count <= GameManager.Get().maxTurrets - 1)
                {
                    GameManager.Get().towersPlaced++;

                    GameObject newTurret = Instantiate(turretTemplate, hit.point + (hit.normal * -5), newTurretPreview.transform.rotation);
                    newTurret.SetActive(true);
                    spawnedTurrets.Add(newTurret);

                    if(OnSpawnerSpawnTurret != null)
                    {
                        OnSpawnerSpawnTurret();
                    }

                    Turret currentTurretProperties = newTurret.GetComponent<Turret>();
                    List<UITowersState> state = GameManager.Get().towersUI;

                    currentTurretProperties.OnTurretDead = DeleteTurretTimer;
                    currentTurretProperties.fireRate = fireRate;
                    currentTurretProperties.attachedModel.material.shader = spawnShader;

                    for (int i = state.Count-1; i >= 0; i--)
                    {
                        if (!state[i].isBeingUsed)
                        {
                            currentTurretProperties.stateUI = state[i];
                            state[i].assignedTurret = currentTurretProperties;
                            state[i].isBeingUsed = true;
                            i = -1;
                        }
                    }
                }
                
            }
        }
    }

    private void DeleteTurret()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 999, deleteTurretMask))
        {
            
            if (hit.transform.gameObject.tag == "turret")
            {

                GameObject turretToDelete = null;

                foreach (GameObject turret in spawnedTurrets)
                {
                    if(turret == hit.transform.gameObject)
                    {
                        turretToDelete = turret;
                    }
                }
                
                if(turretToDelete)
                {
                    spawnedTurrets.Remove(turretToDelete);
                    Destroy(turretToDelete);

                    if (OnSpawnerDeleteTurret != null)
                    {
                        OnSpawnerDeleteTurret();
                    }

                    newTurretPreview.GetComponent<Turret>().enteredTurrets.Remove(turretToDelete);
                    newTurretPreview.GetComponent<Turret>().CheckIfCanBePlaced();
                }
                
            }

            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 999, Color.white);
        }
    }

    private void DeleteTurretTimer(GameObject turret)
    {
        spawnedTurrets.Remove(turret);

        if (OnSpawnerDeleteTurret != null)
        {
            OnSpawnerDeleteTurret();
        }
    }

    private void PreviewTurret()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 999, Mask))
        {

            if (hit.transform.gameObject.tag != "turret")
            {
                newTurretPreview.transform.position = hit.point + (hit.normal * -3.0f);
            }
        }
        else
        {
            newTurretPreview.transform.position = ray.origin * -3;
        }

        if (turretProperties.canBePlaced && turretProperties.isInTurretZone)
        {
            material.SetColor("_BaseColor", Color.green);
            turretMaterial.SetPropertyBlock(material);
        }
        else
        {
            material.SetColor("_BaseColor", Color.red);
            turretMaterial.SetPropertyBlock(material);
        }
    }

    public void SwitchPreview()
    {
        canSpawn = !GameManager.Get().shoot.isActivated;

        if(!canSpawn)
        {
            GameManager.Get().SwitchMeteorActivation();
        }

        canSpawn = !GameManager.Get().shoot.isActivated;
        newTurretPreview.SetActive(!newTurretPreview.activeSelf);
        preview = !preview;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);

        if (preview)
        {
            if(turretButton)
            {
                turretButton.image.color = Color.green;
            }
            
        }
        else
        {
            if(turretButton)
            {
                turretButton.image.color = Color.white;
            }
            
            newTurretPreview.GetComponent<Turret>().enteredZones.Clear();
            newTurretPreview.GetComponent<Turret>().enteredTurrets.Clear();
            newTurretPreview.GetComponent<Turret>().isInTurretZone = false;
            GameManager.Get().SwitchMeteorActivation();
        }
    }

    public void SwitchPreviewForced()
    {
        canSpawn = !canSpawn;
        newTurretPreview.SetActive(!newTurretPreview.activeSelf);
        preview = !preview;
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);

        if (preview)
        {
            turretButton.image.color = Color.green;
        }
        else
        {
            turretButton.image.color = Color.white;
            newTurretPreview.GetComponent<Turret>().enteredZones.Clear();
            newTurretPreview.GetComponent<Turret>().enteredTurrets.Clear();
            newTurretPreview.GetComponent<Turret>().isInTurretZone = false;
        }
    }

    public void StopAllOutlines()
    {
        foreach (GameObject turret in spawnedTurrets)
        {
            turret.GetComponent<Turret>().TurnOffOutline();
        }
    }
}
