using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Queue
{
    public class MarbleQueue : MonoBehaviour
    {
        [SerializeField] private List<MarbleModel> startingQueue;
        [SerializeField] private int maxCapacity;

        private readonly List<Marble> _marbles = new List<Marble>();

        public UnityEvent<Marble> onMarbleEjected;
        public UnityEvent<Marble> onMarbleCreated;

        public int MaxCapacity => maxCapacity;

        private void Start()
        {
            //Initialize the queue
            foreach (MarbleModel model in startingQueue)
            {
                CreateMarble(model);
            }
        }

        private void Update()
        {
            if (_marbles.Count == 0) return;

            for (int i = 0; i < _marbles.Count; i++)
            {
                Marble marble = _marbles[i];
                marble.UpdatePosition(new(0, i, 0));
            }
        }

        private Marble CreateMarble(MarbleModel model)
        {
            //Can we add another marble?
            if (_marbles.Count >= maxCapacity)
            {
                Debug.LogError($"Trying to add {model} to the queue at max capacity");
                return null;
            }

            //Create a new marble
            Marble marble = model.Create();
            AddToTop(marble);


            onMarbleCreated?.Invoke(marble);
            return marble;
        }

        private void AddToTop(Marble marble)
        {
            //Add it to the list
            _marbles.Add(marble);
            //Set its position to the top of the container
            marble.Position = new(0, maxCapacity, 0);
        }


        public Marble EjectMarble()
        {
            Marble marble = _marbles.First();
            _marbles.Remove(marble);
            onMarbleEjected?.Invoke(marble);
            AddToTop(marble);
            return marble;
        }

        public bool IsEmpty()
        {
            return _marbles.IsEmpty();
        }
    }
}