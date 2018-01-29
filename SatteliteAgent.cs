using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatteliteAgent : Agent {

    [Header("Specific to Sattelite")]
    public Player player;
    public Position _position;
    public float prevSpeed = 0f;
    public float actualSpeed = 0f;
    public float prevDelta = 0f;
    public float actualDelta = 0f;

    public override List<float> CollectState()
	{
		List<float> state = new List<float>();
        state.Add(gameObject.transform.localEulerAngles.z);
        return state;
	}

    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            actualSpeed = player.rig.angularVelocity.magnitude;

            if (gameObject.transform.localEulerAngles.z > 180f)
                actualDelta = 360f - gameObject.transform.localEulerAngles.z;
            else
                actualDelta = 180f - gameObject.transform.localEulerAngles.z;

            float action_z = act[0];

            if (action_z < 0)
            {
                player.force[2].enabled = true;
                player.particles[1].Play();
                player.force[1].enabled = false;
                player.particles[0].Stop();
            }
            else if (action_z > 0)
            {
                player.force[1].enabled = true;
                player.particles[0].Play();
                player.force[2].enabled = false;
                player.particles[1].Stop();
            }
            else
            {
                player.force[2].enabled = false;
                player.particles[1].Stop();
                player.force[1].enabled = false;
                player.particles[0].Stop();
            }
        }

        if (_position.position)
            reward = 0.1f;

        if (_position.calibrated)
        {
            reward = 1f;
            done = true;
        }

        if (actualSpeed >= prevSpeed && actualDelta >= prevDelta && !_position.position)
            reward = -0.1f;

        if (actualSpeed < prevSpeed && actualDelta < prevDelta && !_position.position)
            reward = 0.1f;

        if (gameObject.transform.localEulerAngles.z <= 180f)
        {
            if (actualDelta > prevDelta && act[0] < 0)
                reward = 0.1f;

            if (actualDelta > prevDelta && act[0] > 0)
                reward = -0.1f;
        }
        else
        {
            if (actualDelta > prevDelta && act[0] > 0)
                reward = 0.1f;

            if (actualDelta > prevDelta && act[0] < 0)
                reward = -0.1f;
        }

        prevSpeed = actualSpeed;
        prevDelta = actualDelta;
    }

	public override void AgentReset()
	{

	}

	public override void AgentOnDone()
	{

	}
}
