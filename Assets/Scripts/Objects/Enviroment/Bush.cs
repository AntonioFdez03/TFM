using UnityEngine;

public class Bush : HarvestableObject
{   
    protected override void Awake()
    {
        base.Awake();
        objectName = "Bush";
        maxHealth = 30;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);
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

        DropItem();
        DropItem();
        DropItem();

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