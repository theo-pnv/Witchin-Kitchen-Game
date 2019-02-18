﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using con2.game;
using con2.messages;
using Newtonsoft.Json;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Event = con2.messages.Event;

namespace con2
{

    /// <summary>
    /// For communication happening in the game
    /// </summary>
    public partial class AudienceInteractionManager : MonoBehaviour
    {
        [HideInInspector]
        public Dictionary<Events.EventID, List<IEventSubscriber>> EventSubscribers;

        void GameStart()
        {
            _Socket.On(Command.RECEIVE_VOTES, OnReceiveEventVotes);
        }

        #region Emit

        public void SendPoll(PollChoices pollChoices)
        {
            Debug.Log("SendPoll");
            var serialized = JsonConvert.SerializeObject(pollChoices);
            _Socket.Emit(Command.LAUNCH_POLL, new JSONObject(serialized));
        }

        #endregion

        #region Receive

        private void OnGameMessage(Base content)
        {
            if ((int)content.code % 10 == 0) // Success codes always have their unit number equal to 0 (cf. protocol)
            {
                Debug.Log(content.content);
                switch (content.code)
                {
                    default: break;
                }
            }
            else
            {
                Debug.LogError(content.content);
            }
        }

        private void OnReceiveEventVotes(SocketIOEvent e)
        {
            var content = JsonConvert.DeserializeObject<PollChoices>(e.data.ToString());
            var voteA = content.events[0];
            var voteB = content.events[1];
            Debug.Log("Votes for A: " + voteA.votes);
            Debug.Log("Votes for B: " + voteB.votes);

            var mostVoted = voteA.votes > voteB.votes ? voteA : voteB;
            EventSubscribers[(Events.EventID)mostVoted.id]
                .ForEach((subscriber) =>
            {
                subscriber.ActivateEventMode();
            });
        }

        #endregion

    }

}
