using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;

    //Variables de velocidad.
    [Header("Player")]
    public float speed;
    public GameObject crosshairAim, crosshairPistol;
    public Gun activeGun;
    public Image imageUI;

    //Equipo del personaje.
    [Header("Guns")]
    public List<Gun> allGuns = new List<Gun>();
    public int currentGun;
    public Transform firePoint, firePoint2;
    public float reloadTime;
    public bool isReloading;

    //Movimiento de la cámara.
    [Header("Camera")]
    public Transform camTrans;
    public float mouseSensitivityX, mouseSensitivityY;
    public float maxViewAngle;

    Animator anim;

    PhotonView PV;
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>());
            Destroy(rb);
        }
        else
        {
            CameraController.instance.StartDelay(camTrans);
            GameManager.instance.player = gameObject;

            anim = GetComponent<Animator>();
            activeGun = allGuns[currentGun];
            activeGun.gameObject.SetActive(true);

            UIController.instance.ammoText.text = activeGun.currentAmmo + "/" + activeGun.maxAmmo;
            imageUI = GameObject.Find("AmmoImage").GetComponent<Image>();
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //Función de movimiento del personaje.
        transform.Translate(v * Vector3.forward * speed * Time.deltaTime);
        transform.Translate(h * Vector3.right * speed * Time.deltaTime);

        //Función de rotación de la camara con el mouse.
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") *mouseSensitivityX, Input.GetAxisRaw("Mouse Y") * mouseSensitivityY);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0, 0));

        if (camTrans.rotation.eulerAngles.x > maxViewAngle && camTrans.rotation.eulerAngles.x < 180)
        {
            camTrans.rotation = Quaternion.Euler(maxViewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
        }
        else if (camTrans.rotation.eulerAngles.x > 180 && camTrans.rotation.eulerAngles.x < 360 - maxViewAngle)
        {
            camTrans.rotation = Quaternion.Euler(-maxViewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
        }

        anim.SetFloat("VelH", h);
        anim.SetFloat("VelV", v);

        //Disparo con arco.
        if (currentGun == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && activeGun.fireCounter <= 0 && !isReloading) //Si presionas el click izquierdo...
            {
                RaycastHit hit;
                if (Physics.Raycast(camTrans.transform.position, camTrans.transform.forward, out hit, 50f))
                {
                    if (Vector3.Distance(camTrans.position, hit.point) > 2f)
                    {
                        firePoint.LookAt(hit.point);
                    }
                }
                FireShot();
            }
            else
            {
                firePoint.LookAt(camTrans.position + (camTrans.forward) * 30);
            }
        }
        //Disparo con pistola.
        if (currentGun == 1)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && activeGun.fireCounter <= 0 && !isReloading) //Si presionas el click izquierdo...
            {
                RaycastHit hit;
                if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
                {
                    if (Vector3.Distance(camTrans.position, hit.point) > 2f)
                    {
                        firePoint2.LookAt(hit.point);
                    }
                }
                FireShot2();
            }
            else
            {
                firePoint2.LookAt(camTrans.position + (camTrans.forward) * 30);
            }
        }
        //Recargar municion.
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
        //Cambio de Arco a pistola.
        if (Input.GetButtonDown("Switch Gun"))
        {
            SwitchGun();
            crosshairPistol.SetActive(true);
        }
        //Cambio de Pistola a arco.
        if (Input.GetButtonDown("Switch Gun 2"))
        {
            SwitchGun2();
            crosshairPistol.SetActive(false);
        }
        //FOV para aimear.
        if (Input.GetMouseButtonDown(1))
        {
            CameraController.instance.ZoomIn(activeGun.zoomAmount);
        }
        //FOV general.
        if (Input.GetMouseButtonUp(1))
        {
            CameraController.instance.ZoomOut();
        }

        Animations();
    }
    void Animations()
    {
        //Correr con el shift.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            anim.SetBool("IsRunning", true);
        else
            anim.SetBool("IsRunning", false);
            
        //Saltar con el espacio.
        if (Input.GetKey(KeyCode.Space))
            anim.SetBool("Jump", true);
        else
            anim.SetBool("Jump", false);

        //Agacharse con el control.
        if (Input.GetKey(KeyCode.LeftControl))
            anim.SetBool("Crouch", true);
        else
            anim.SetBool("Crouch", false);

        //Movimiento con el personaje apuntando.
        if (Input.GetKey(KeyCode.Mouse1) && !isReloading)
        {
            anim.SetBool("AimMove", true);
            crosshairAim.SetActive(true);
        }
        else
        {
            anim.SetBool("AimMove", false);
            crosshairAim.SetActive(false);
        }

        //Disparar con el click izquierdo.
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading)
            anim.SetBool("Attack", true);
        else
            anim.SetBool("Attack", false);

        //Dodgear con el segundo botón lateral del ratón.
        if (Input.GetKey(KeyCode.Mouse4))
            anim.SetBool("Dodge", true);
        else
            anim.SetBool("Dodge", false);

        //Cambiar arma a la Q.
        if (Input.GetKey(KeyCode.Q))
            anim.SetBool("ChangeWeapon", true);
        else
            anim.SetBool("ChangeWeapon", false);

        //Equipar el arco al 1.
        if (Input.GetKey(KeyCode.Alpha1))
        {
            anim.SetBool("EquipBow", true);
        }
        else
            anim.SetBool("EquipBow", false);       
    }
    public void FireShot()
    {
        if(activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate(activeGun.ammo, firePoint.position, firePoint.rotation); //Lanza una flecha en el punto que hemos definido previamente.
            activeGun.fireCounter = activeGun.fireRate;
            UIController.instance.ammoText.text = activeGun.currentAmmo + "/" + activeGun.maxAmmo;
        }
    }
    public void FireShot2()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate(activeGun.ammo, firePoint2.position, firePoint2.rotation); //Lanza una bala en el punto que hemos definido previamente.
            activeGun.fireCounter = activeGun.fireRate;
            UIController.instance.ammoText.text = activeGun.currentAmmo + "/" + activeGun.maxAmmo;
        }
    }
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        activeGun.currentAmmo = activeGun.maxAmmo;
        UIController.instance.ammoText.text = activeGun.currentAmmo + "/" + activeGun.maxAmmo;
        isReloading = false;
    }
    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);

        currentGun++;

        if (currentGun >= allGuns.Count)
        {
            currentGun--;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = activeGun.currentAmmo + "/" + activeGun.maxAmmo;
        imageUI.sprite = Resources.Load<Sprite>("Sprites/Icono Ammo");
    }
    public void SwitchGun2()
    {
        activeGun.gameObject.SetActive(false);

        currentGun--;

        if (currentGun <= allGuns.Count)
        {
            currentGun = 0;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = activeGun.currentAmmo + "/" + activeGun.maxAmmo;
        imageUI.sprite = Resources.Load<Sprite>("Sprites/Icono Arrow");
    }
}
