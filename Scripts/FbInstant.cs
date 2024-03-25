namespace UniT.FbInstant
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using UnityEngine;
    using UnityEngine.Scripting;

    public static partial class FbInstant
    {
        private sealed class This : MonoBehaviour
        {
            private const string CALLBACK_OBJ    = nameof(FbInstant);
            private const string CALLBACK_METHOD = nameof(Callback);

            static This() => DontDestroyOnLoad(new GameObject(CALLBACK_OBJ).AddComponent<This>());

            private static readonly Dictionary<string, UniTaskCompletionSource<Result<string>>> Tcs = new Dictionary<string, UniTaskCompletionSource<Result<string>>>();

            public static UniTask<Result<string>> Invoke(object data, Action<string> action) => Invoke(JsonConvert.SerializeObject(data), action);

            public static UniTask<Result<string>> Invoke(string data, Action<string> action) => Invoke((callbackObj, callbackMethod, callbackId) => action(data, callbackObj, callbackMethod, callbackId));

            public static async UniTask<Result<string>> Invoke(Action action)
            {
                var callbackId = Guid.NewGuid().ToString();
                Tcs.Add(callbackId, new UniTaskCompletionSource<Result<string>>());
                try
                {
                    action(CALLBACK_OBJ, CALLBACK_METHOD, callbackId);
                    return await Tcs[callbackId].Task;
                }
                finally
                {
                    Tcs.Remove(callbackId);
                }
            }

            private void Callback(string json)
            {
                var message = JsonConvert.DeserializeObject<Message>(json);
                Tcs[message.CallbackId].TrySetResult(message);
            }

            [Preserve]
            private sealed class Message : Result<string>
            {
                public string CallbackId { get; }

                [JsonConstructor]
                public Message(string data, string error, string callbackId) : base(data, error)
                {
                    this.CallbackId = callbackId;
                }
            }
        }

        private static UniTask<Result<T>> Convert<T>(this UniTask<Result<string>> task)
        {
            return task.ContinueWith(result => new Result<T>(JsonConvert.DeserializeObject<T>(result.Data), result.Error));
        }

        private static UniTask<Result> WithErrorOnly(this UniTask<Result<string>> task)
        {
            return task.ContinueWith(result => (Result)result);
        }

        private delegate void Action(string callbackObj, string callbackMethod, string callbackId);

        private delegate void Action<in T>(T data, string callbackObj, string callbackMethod, string callbackId);
    }
}