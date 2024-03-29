﻿using Assets.Scripts.Core.ECS;
using Assets.Scripts.Core.ECS.Interfaces;
using Assets.Scripts.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Engine.ECS
{
    public interface IEntity : IActivable
    {
        Guid ID { get; }
        List<ITag> Tags { get; }

        Archetype Archetype { get; }

        IComponent AddComponent(IComponent component);
        IEntity Clone();
        bool Equals(object obj);
        T GetComponent<T>() where T : IComponent;
        int ComponentsCount { get; }
        IComponent[] GetComponents();
        T[] GetComponents<T>() where T : IComponent;
        int GetHashCode();
        bool HasComponent<T>() where T : IComponent;
        bool HasComponent<T1, T2>() where T1 : IComponent where T2 : IComponent;
        bool HasComponent<T1, T2, T3>() where T1 : IComponent where T2 : IComponent where T3 : IComponent;
        bool HasComponent<T1, T2, T3, T4>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent;
        bool HasComponent<T1, T2, T3, T4, T5>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent where T5 : IComponent;
        void AddTag(ITag tag);
        bool HasTag(string tag);
        void RemoveTag(ITag tag);
        void RemoveComponent<T>() where T : IComponent;
        string ToString();
        bool TryGetComponent<T>(out T component) where T : IComponent;
        bool HasSameComposition(IEntity entity);
        bool HasSameComposition(IComponent[] components, string[] tags);
        bool HasComponent(IComponent component);
        void RemoveComponent(DataComponent dataComponent);
        void Initialize(Archetype archetype);
    }
}