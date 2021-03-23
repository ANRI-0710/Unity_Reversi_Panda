using UnityEngine;
/// <summary>
/// パンダの動きを管理するクラス（秒間でランダムな方向に歩く・餌を食べるなどのアクション）
/// </summary>
public class Panda_Animation : MonoBehaviour
{
    [SerializeField]
    private GameObject _PandaPrefab;

    [SerializeField]
    private GameObject _DirectionBox;   //ランダムで歩くパンダの方向を定める。透明なボックス

    [SerializeField]
    private GameObject Esa;
   
    private Animator _Animator;
    private Rigidbody _Rbody;

    //時間制御
    private delegate bool Time_count(float a);
    private delegate bool One_Time_Count();

    private bool _DirectionCount;
    private bool _One_Time;

    private float _TimeCount;
    private float _Esa_time;
    private int _Esa_Count;

    //行動制御専用関数
    private delegate void PandaAction();

    PandaAction DirectionAction;
    PandaAction WalkAction;
       
    void Start()
    {
        _Animator = GetComponent<Animator>();
        _Rbody = GetComponent<Rigidbody>();

        WalkAction = Panda_Walk;
        DirectionAction = Panda_direction;

        Esa.GetComponent<MeshRenderer>().enabled = false;
        DirectionAction();
        One_Time();

        _Esa_Count = PlayerPrefs.GetInt(SaveData_Manager.KEY_PANDA_ESA, 0);
    }

    void Update()
    {      
        WalkAction();
        _DirectionCount = TimeCountTrigger(10f);

        if (_DirectionCount)
        {
             DirectionAction();
            _DirectionCount = false;
        }
    }

    /// <summary>
    /// 秒間ごとにtrueを返す。（x秒間ランダムに歩き続けさせる）
    /// </summary>
    /// <param name="time">秒数指定</param>
    /// <returns></returns>
    private bool TimeCountTrigger(float time)
    {
        _TimeCount += Time.deltaTime;
        var timecount = false;

        if (_TimeCount > time)
        {
            _TimeCount = 0;
            timecount = true;
            Debug.Log(time + "秒経過");
        }
        return timecount;
    }

    /// <summary>
    /// 1フレーム目をカウントする
    /// </summary>
    /// <returns></returns>
    private bool One_Time()
    {
        if (!_One_Time)
        {
            Debug.Log("秒経過");
            DirectionAction();
            _One_Time = true;

        }
        return _One_Time;
    }

    /// <summary>
    /// パンダの餌オブジェクトをオンして餌やりモードにする
    /// </summary>
    public void Esa_Start()
    {
        _Esa_Count = PlayerPrefs.GetInt(SaveData_Manager.KEY_PANDA_ESA, 0);
        if (_Esa_Count > 0)
        {
            Esa.SetActive(true);
            Esa.GetComponent<MeshRenderer>().enabled = true;
            DirectionAction = Esa_Throw;
        }
    }

    /// <summary>
    /// パンダを歩かせる
    /// </summary>
    public void Panda_Walk()
    {
        _Animator.SetBool("Walk", true);
        _Rbody.AddForce(transform.forward * 1000f);
    }

    /// <summary>
    /// ランダムボックス、もしくは餌にぶつかったら歩きを止める
    /// </summary>
    public void Panda_Stop()
    {
        _DirectionBox.SetActive(true);
        _Esa_time += Time.deltaTime;
        _Animator.SetBool("Walk", false);

        //餌を投げている間は、餌の方向にパンダが歩く
        if (DirectionAction == Esa_Throw)
        {
            DirectionAction = Esa_Throw;
            WalkAction = Panda_Walk;
            _Esa_time = 0;

        }//パンダが止まっている状態で_Esa_Timeが10秒超えたら、通常モードに戻す
        else if (WalkAction == Panda_Stop && _Esa_time > 10.0f)
        {
            WalkAction = Panda_Walk;
            DirectionAction = Panda_direction;
            DirectionAction();

            _Esa_time = 0;

        }

    }

    /// <summary>
    /// パンダに餌をやった場合、餌を食べるモーションを開始し、10秒経ったら通常モードに戻す
    /// </summary>
    public void Panda_Esa()
    {
        _Animator.SetBool("Walk", false);
        _Animator.SetBool("Esa", true);
        _Esa_time += Time.deltaTime;

        if (_Esa_time > 10.0f)
        {
            _Animator.SetBool("Walk", true);
            _Animator.SetBool("Esa", false);
            _DirectionBox.SetActive(true);

            WalkAction = Panda_Walk;
            DirectionAction = Panda_direction;
            Esa.GetComponent<MeshRenderer>().enabled = false;

            var Panda_like = PlayerPrefs.GetInt("PANDALIKE", 0);

            Panda_like++;
            Debug.Log("好感度" + Panda_like);
            PlayerPrefs.SetInt("PANDALIKE", Panda_like);
            _Esa_time = 0;

        }

    }

    /// <summary>
    /// パンダがランダムな方向に動く
    /// </summary>
    public void Panda_direction()
    {
        var x_r = Random.Range(550, 820);
        var z_r = Random.Range(420, 730);
        var pos_y = 105f;

        _DirectionBox.transform.localPosition = new Vector3(x_r, pos_y, z_r);

        var aim = this._DirectionBox.transform.localPosition - this.transform.localPosition;
        var look = Quaternion.LookRotation(aim);
        this.transform.localRotation = look;

        Debug.Log("Panda_direction");

    }

    /// <summary>
    /// パンダを餌の方向に向け前進させる
    /// </summary>
    public void Esa_Throw()
    {
        _DirectionBox.SetActive(false);
        _DirectionCount = false;
        var aim = Esa.transform.localPosition - this.transform.localPosition;
        var look = Quaternion.LookRotation(aim);

        this.transform.localRotation = look;

        Debug.Log("Esa_Nageru");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("box"))
        {
            WalkAction = Panda_Stop;
            Debug.Log("ランダムボックスに接触");
        }

        if (collision.gameObject.CompareTag("Esa"))
        {
            WalkAction = Panda_Esa;
            Debug.Log("Esaに接触");
        }
    }

}