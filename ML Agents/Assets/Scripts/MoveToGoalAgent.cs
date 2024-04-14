using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoalAgent : Agent
{
    [SerializeField] Transform m_targetTransfrom;
    [SerializeField] float m_moveSpeed;
    [SerializeField] Material m_winMaterial;
    [SerializeField] Material m_loseMaterial;
    [SerializeField] MeshRenderer m_floorMeshRenderer;
    public override void OnEpisodeBegin()
    {
        transform.rotation = Quaternion.identity;
        transform.localPosition = new Vector3(Random.Range(-4f, 0f), 0, Random.Range(-4f, 4f));
        m_targetTransfrom.localPosition = new Vector3(Random.Range(1, 4f), 0, Random.Range(-4f, 4f));
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * m_moveSpeed;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(m_targetTransfrom.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            m_floorMeshRenderer.material = m_winMaterial;
            EndEpisode();
        }
        else if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            m_floorMeshRenderer.material = m_loseMaterial;
            EndEpisode();
        }
    }
}
