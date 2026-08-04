using UnityEngine;

public class CrossPoint : MonoBehaviour
{
    public Transform[] straight;
    public Transform[] left;
    public Transform[] right;


    private void OnTriggerEnter(Collider other)
    {
        Car car = other.GetComponent<Car>();

        if (car != null)
        {
            int type = Random.Range(0, 3);


            switch (type)
            {
                // 直進
                case 0:
                    car.SetRoute(straight, 0);
                    //Debug.Log("直進");
                    break;


                // 左折
                case 1:
                    car.SetRoute(left, -90);
                    //Debug.Log("左折");
                    break;


                // 右折
                case 2:
                    car.SetRoute(right, 90);
                    //Debug.Log("右折");
                    break;
            }
        }
    }
}