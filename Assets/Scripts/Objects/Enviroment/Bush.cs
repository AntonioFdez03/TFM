using UnityEngine;

public class Bush : HarvestableObject
{   
    private float recolectTime = 5f;
    private float timer = 0;
    private bool recolected = false;
    private int dropCount = 3;

    protected override void Awake()
    {
        base.Awake();
        objectName = "Bush";
        maxHealth = 30;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);
    }

    public void SetCurrentTime(float t) => timer = t;
    public float GetCurrentTime() => timer;
    public float GetRecolectTime() => recolectTime;

    public void Recolect()
    {
        timer += Time.deltaTime;

        if(timer >= recolectTime)
        {   
            recolected = true;
            Harvest();
            timer = 0;
        }
    }
    public override void Harvest()
    {
        int berriesCount = transform.childCount;

        for (int i = 1; i < berriesCount; i++)
        {
            Transform berry = transform.GetChild(1);

            print("berry " + i + " de " + berriesCount);

            berry.SetParent(InventoryController.instance.GetItemsParent());

            Rigidbody rb = berry.GetComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.down, ForceMode.Impulse);
        }

        if (recolected)
        {
            for(int i = 0; i < dropCount; i++)
            {
                GameObject dropInstance = Instantiate(dropItem);
                if(!InventoryController.instance.AddItem(dropInstance))
                    DropItem();
            }
        }
        else
        {
            for(int i = 0; i < dropCount; i++)
            {
                DropItem();
            }
        }

        Destroy(gameObject);
    }

    private void DropItem()
    {
        Vector3 dropPosition = transform.position + Vector3.up * 2;

        GameObject dropItemInstance = Instantiate(dropItem, dropPosition, Quaternion.identity);

        dropItemInstance.transform.SetParent(InventoryController.instance.GetItemsParent());

        Rigidbody dropItemRB = dropItemInstance.GetComponent<Rigidbody>();
        if(dropItemRB != null)
        {
            // Genera una dirección aleatoria horizontal
            Vector3 randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)).normalized;

            float forceMagnitude = Random.Range(50f, 100f);
            dropItemRB.isKinematic = false;
            dropItemRB.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
        }
    }
}