using TMPro;
using UnityEngine;

public enum ClassFeatureType
{
    Property,
    Method
}

public class ClassFeatureManager : MonoBehaviour
{
    public ClassFeatureType featureType;

    public TextMeshProUGUI featureText;

    private GameObject umlClass;

    public GameObject backgroundPlane;

    public delegate void UmlClassFeatureGrabEvent(ClassFeatureType type);
    public static event UmlClassFeatureGrabEvent OnClassFeatureGrabbed;

    public static event UmlClassFeatureGrabEvent OnClassFeaturePlaced;

    public void Init(string featureText, ClassFeatureType featureType)
    {
        this.featureText.text = featureText;
        this.featureType = featureType;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UmlClassCanvas"))
        {
            umlClass = other.gameObject;
            umlClass.transform.parent.GetComponent<Outline>().enabled = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UmlClassCanvas"))
        {
            umlClass.transform.parent.GetComponent<Outline>().enabled = false;
            umlClass = null;
        }
    }

    public void OnRelease()
    {
        if (umlClass != null)
        {
            if (featureType == ClassFeatureType.Property
                && UmlHangmanLevelValidator.Instance.ValidatePropertyAssignment(
                    featureText.text,
                    umlClass.transform.parent.GetComponent<UmlClassStructure>().Name
                )
            )
            {
                // umlClass.transform.parent // get top-level UmlClass object to which the RPC manager is attached
                //     .GetComponent<UmlClassRpcManager>()
                //     .DoRPCCallToAll("AddClassFeature", GetComponent<PhotonView>().ViewID, featureText.text, featureType);
                umlClass.transform.parent.GetComponent<UmlClassStructure>().AddClassFeature(featureText.text, featureType);
                UmlHangmanLevelValidator.Instance.PlayerSoundManager.PlayCorrectMoveSound();
                OnClassFeaturePlaced?.Invoke(featureType);

                gameObject.SetActive(false);
            }
            else if (featureType == ClassFeatureType.Method
              && UmlHangmanLevelValidator.Instance.ValidateMethodAssignment(
                  featureText.text,
                  umlClass.transform.parent.GetComponent<UmlClassStructure>().Name
              )
            )
            {
                umlClass.transform.parent.GetComponent<UmlClassStructure>().AddClassFeature(featureText.text, featureType);
                UmlHangmanLevelValidator.Instance.PlayerSoundManager.PlayCorrectMoveSound();
                OnClassFeaturePlaced?.Invoke(featureType);

                gameObject.SetActive(false);
            }
            else
            {
                UmlHangmanLevelValidator.Instance.PlayerSoundManager.PlayWrongMoveSound();
            }

            umlClass.transform.parent.GetComponent<Outline>().enabled = false;
        }
    }

    // public void OnPhotonInstantiate(PhotonMessageInfo info)
    // {
    //     if (info.photonView.InstantiationData?.Length >= 2)
    //     {
    //         var type = info.photonView.InstantiationData[1].ToString().Equals("Property")
    //         ? ClassFeatureType.Property
    //         : ClassFeatureType.Method;
    //         Init(info.photonView.InstantiationData[0] as string, type);
    //     }
    // }

    public void InvokeOnFeatureGrabbed()
    {
        OnClassFeatureGrabbed?.Invoke(featureType);
    }
}
