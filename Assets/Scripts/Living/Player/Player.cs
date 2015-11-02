using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(WeaponController))]
public class Player : LivingEntity {

    public float moveSpeed = 5;
    PlayerController controller;
    Camera viewCamera;
    WeaponController weaponController;

    protected override void Start() {
        base.Start();
        controller = GetComponent<PlayerController>();
        weaponController = GetComponent<WeaponController>();
        viewCamera = Camera.main;
	}
	
	void Update () {
        //Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelociry = moveInput.normalized * moveSpeed;
        controller.Move(moveVelociry);

        //Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        //Weapon input
        if (Input.GetMouseButton(0))
        {
            weaponController.Shoot();
        }

	}
}
