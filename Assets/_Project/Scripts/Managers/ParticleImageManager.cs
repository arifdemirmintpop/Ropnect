using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetKits.ParticleImage;
using System;
using DG.Tweening;
using tiplay.MoneyKit;

public class ParticleImageManager : MonoSingleton<ParticleImageManager>
{
    [SerializeField] ParticleImage particleImagePrefab;

    [SerializeField] Transform parent;

    [SerializeField] ParticleImageStruct[] particleImageStructs;

    public void CreateParticleImage(Transform position, ParticleImageType type, float value, Action lastAction, Transform _parent = null)
    {
        Transform fixedParent = _parent != null ? _parent : parent;

        ParticleImage particleImage = Instantiate(particleImagePrefab, fixedParent);

        var particleInfo = GetParticleInfo(type, value);

        particleImage.attractorTarget = particleInfo.attractorTarget;

        particleImage.sprite = particleInfo.sprite;

        particleImage.rectTransform.position = position.GetComponent<RectTransform>().position;

        particleImage.onFirstParticleFinished.AddListener(() =>
        {
            if (particleInfo.firstAction != null)
            {
                particleInfo.firstAction.Invoke();
            }
        }
        );
        if (lastAction != null)
        {
            particleImage.onLastParticleFinished.AddListener(() =>
            {
                lastAction.Invoke();
            });
        }

        particleImage.onAnyParticleFinished.AddListener(() =>
        {
            particleImage.attractorTarget.transform.DOComplete();
            particleImage.attractorTarget.transform.DOPunchScale(Vector3.one * .5f, .5f);
        }
        );



    }
    

    private ParticleImageStruct GetParticleInfo(ParticleImageType _type, float value)
    {
        for (int i = 0; i < particleImageStructs.Length; i++)
        {
            var particle = particleImageStructs[i];

            if (particle.type == ParticleImageType.Coin) 
            {
                particleImageStructs[i].firstAction = ()=> Money.IncreaseMoney(value, particle.name);
            }
            
        }

        foreach (var item in particleImageStructs)
        {
            if (item.type == _type)
            {
                return item;
            }
        }
        return null;
    }

    [Serializable]
    public class ParticleImageStruct
    {
        public string name;
        public Sprite sprite;
        public ParticleImageType type;
        public Transform attractorTarget;

        public Action firstAction;

    }
    
}
