using UnityEngine;
/// <summary>
/// タイトルシーンの走っているパンダの動きを管理するクラス
/// </summary>
public class TItle_Panda_Animation : MonoBehaviour
{
    private Rigidbody _rbody;

    private Vector3 PandaPosition;

    private float Panda_Run_Distance = 550f;

    private float Panda_Speed = 1000f;

    private void Start()
    {
        _rbody = GetComponent<Rigidbody>();
        PandaPosition = this.transform.position;
    }

    void Update()
    {
        var pandapotision_x = this.transform.position.x;

        if (pandapotision_x < Panda_Run_Distance) {

            this.transform.position = PandaPosition;        
        }
        _rbody.AddForce(transform.forward * Panda_Speed);
    }
}
