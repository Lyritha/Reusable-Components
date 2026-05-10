using UnityEngine;

public class PlasticExplosivePlacer : PlaceableWeapon
{
    [SerializeField]
    private PlasticExplosive plasticExplosivePrefab;

    private GameObject placeholderObj;

    private bool CanPlace;
    private Vector3 placePos;
    private Quaternion placeRot;
    private Vector3 hitNormal;

    private void Awake()
    {
        AddSubscription(e => e.OnPrimaryMouse += Place, e => e.OnPrimaryMouse -= Place);

        // create clone to show where the explosive would be placed
        placeholderObj = GhostUtility.CreateGhost(plasticExplosivePrefab, gameObject.transform, false);

        ContinuesRaycast.EnsureExistence();
        ContinuesRaycast.OnRayHit += OnRayHit;
        ContinuesRaycast.OnRayExited += OnRayExited;

    }

    private void OnRayHit(RaycastHit hit)
    {
        CanPlace = true;
        placePos = hit.point;
        placeRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        hitNormal = hit.normal;

        placeholderObj.SetActive(true);
        placeholderObj.transform.SetPositionAndRotation(placePos, placeRot);
    }

    private void OnRayExited()
    {
        CanPlace = false;
        placeholderObj.SetActive(false);
    }

    private void Place(bool started)
    {
        if (!CanPlace || !started) return;

        PlasticExplosive explosive = Instantiate(plasticExplosivePrefab, placePos, placeRot);
        explosive.Initialize(hitNormal);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        ContinuesRaycast.OnRayHit -= OnRayHit;
        ContinuesRaycast.OnRayExited -= OnRayExited;
    }
}