using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemEffectTimer : MonoBehaviour
{
    Character character;
    Resource.Effect effect;

    public string instanceID;

    [SerializeField] public int timeLeft;

    void Start()
    {
        Initiate();

    }

    void Update()
    {
        timeLeft = (int)((effect.finishTime - DateTime.Now.Ticks) / TimeSpan.TicksPerSecond);
        if (CheckCompleteTimer())
        {
            return;

        }

    }

    bool CheckCompleteTimer()
    {
        if (effect.finishTime <= DateTime.Now.Ticks)
        {
            StopEffect();
            return true;

        }
        return false;

    }

    public void StartEffect(Character _character, Resource.Effect _effect)
    {
        character = _character;

        effect =  _effect;



    }

    void StopEffect()
    {
        character.DecreaseStats(effect.stats);
        character.effects.Remove(effect);
        Destroy(this.gameObject);

        /// Fake Event for Updating Character Canvas Information.
        EventManager.Instance.CharacterAssigned();

    }
    public void IncreaseDuration(int duration)
    {
        effect.finishTime += (duration * TimeSpan.TicksPerSecond);

    }

    private void Initiate()
    {
        

        if (effect.finishTime == 0)
        {

            effect.startTime = DateTime.Now.Ticks;
            effect.finishTime = DateTime.Now.Ticks + (effect.duration * TimeSpan.TicksPerSecond);
            character.IncreaseStats(effect.stats);
            effect.SetInstanceID(Resource.Effect.GenerateInstanceID(character, effect));

            /// Fake Event for Updating Character Canvas Information.
            EventManager.Instance.CharacterAssigned();

        }

        instanceID = effect.instanceID;
        gameObject.name = instanceID;

    }

}
