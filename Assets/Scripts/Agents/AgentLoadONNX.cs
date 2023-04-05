using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Barracuda;
using UnityEngine;

public class AgentLoadONNX : MonoBehaviour
{
    [SerializeField]
    private NNModel model;
    [SerializeField]
    private AlphaAgent agent;
    [SerializeField]
    private Prediction prediction;

    private Model runtimeModel;
    private IWorker worker;
    private string outputLayerName;

    [Serializable]
    public struct Prediction {
        [SerializeField]
        private int predictionValue;
        [SerializeField]
        private float[] predictions;

        public void SetPrediction(Tensor t) {
            Debug.Log($"Prediction Tensor Shape: {t.shape}");
            predictions = t.AsFloats();
            predictionValue = Array.IndexOf(predictions, predictions.Max());
            Debug.Log($"Predictions: {predictionValue}");
        }
    }

    private void Start() {
        runtimeModel = ModelLoader.Load(model);
        // var additionalOutputs = new string[] { "99", "60" };
        worker = WorkerFactory.CreateWorker(runtimeModel, GetLayerNames(), WorkerFactory.Device.GPU);
        outputLayerName = runtimeModel.outputs[runtimeModel.outputs.Count - 1];
        prediction = new Prediction();
        GetModelInformation();
    }

    private string[] GetLayerNames() {
        string[] additionalOutputs = new string[runtimeModel.layers.Count];
        for (int i = 0; i < runtimeModel.layers.Count; i++) {
            additionalOutputs[i] = runtimeModel.layers[i].name;
        }

        return additionalOutputs;
    }

    private void GetModelInformation() {
        string shape = "";
        foreach (int size in runtimeModel.inputs[0].shape)
            shape += size + ", ";

        Debug.Log($"Input Name: {runtimeModel.inputs[0].name}");
        Debug.Log($"Input Shape: {shape}");
        Debug.Log($"Observation Shape: {runtimeModel.GetShapeByName("obs_0")}");
        Debug.Log($"ActionMask Shape: {runtimeModel.GetShapeByName("action_masks")}");
    }

    [Button]
    internal void Predict(string inputName = "discrete_actions") {
        /* float[] actionMask = new float[21];
        for (int i = 0; i < actionMask.Length; i++) {
            actionMask[i] = 1f;
        } */

        foreach (var layer in runtimeModel.layers) {
            string info = "Name: " + layer.name + " does " + layer.type + " and has layout: " + " and input is: ";
            if (layer.weights.Length > 0) {
                foreach (float inputString in layer.weights) {
                    info += inputString + ", ";
                }
            }
            Debug.Log(info);
        }

        using Tensor inputTensor = new Tensor(1, 1, 1, 47, agent.SensorToFloatArray(agent.ObservationSensor));
        using Tensor actionMaskTensor = new Tensor(1, 1, 1, 21, agent.GetActionMaskFloatArray());
        Dictionary<string, Tensor> inputDictionary = new Dictionary<string, Tensor>();
        // actionMaskTensor[0] = 0;
        // inputTensor[0] = input;

        inputDictionary.Add("obs_0", inputTensor);
        inputDictionary.Add("action_masks", actionMaskTensor);

        // Tensor outputTensor = worker.Execute(inputDictionary).PeekOutput("deterministic_discrete_actions");
        // Tensor outputTensor = worker.Execute(inputDictionary).PeekOutput("discrete_actions");
        Tensor outputTensor = worker.Execute(inputDictionary).PeekOutput(inputName);

        // Debug.Log("Floats:");
        // foreach (Tensor tensor in outputTensor) {
        //     foreach (float tensorfloat in tensor.AsFloats()) {
        //         Debug.Log(tensorfloat);
        //     }
        // }

        prediction.SetPrediction(outputTensor);
        
        inputTensor.Dispose();
        actionMaskTensor.Dispose();
    }

    private void OnDestroy() {
        worker?.Dispose();
    }
}
