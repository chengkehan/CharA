using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameScript.WaypointSystem;

namespace GameScript
{
    [DisallowMultipleComponent]
    public class ActorsManager : MonoBehaviour
    {
        private static ActorsManager s_instance = null;

        public static ActorsManager GetInstance()
        {
            return s_instance;
        }

        [SerializeField]
        private Transform _root = null;
        private Transform root
        {
            get
            {
                return _root;
            }
        }

        private List<Role> allRoles = new List<Role>();

        public int NumberActors()
        {
            return allRoles.Count;
        }

        public Actor GetActor(int index)
        {
            if (index < 0 || index >= allRoles.Count)
            {
                return null;
            }
            return allRoles[index].actor;
        }

        public Actor GetHeroActor()
        {
            foreach (var role in allRoles)
            {
                if (role.actor.isHero)
                {
                    return role.actor;
                }
            }
            return null;
        }

        public Actor GetActorByGUID(string actorGUID)
        {
            foreach (var role in allRoles)
            {
                if (role.actor.guid == actorGUID)
                {
                    return role.actor;
                }
            }
            return null;
        }

        public Actor GetActor(string roleId)
        {
            foreach (var role in allRoles)
            {
                if (role.roleId == roleId)
                {
                    return role.actor;
                }
            }
            return null;
        }

        public bool ContainsRole(string roleId)
        {
            foreach (var role in allRoles)
            {
                if (role.roleId == roleId)
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadRole(string roleId, Action completeCB)
        {
            AssetsManager.GetInstance().LoadGameObject(AssetsManager.ROLE_ASSET_PREFIX + roleId, (go) =>
            {
                var actor = go.GetComponent<Actor>();

                Utils.Assert(actor != null, "Can't find Actor on a role." + roleId);
                Utils.Assert(ContainsRole(roleId) == false, "Duplicate loading role." + roleId);

                var role = new Role();
                role.roleId = roleId;
                role.actor = actor;
                role.gameObject = go;
                role.gameObject.name = roleId;
                allRoles.Add(role);

                role.actor.AttachToRoot(root);

                completeCB?.Invoke();
            });
        }

        private void Awake()
        {
            s_instance = this;
        }

        private void OnDestroy()
        {
            UnloadAllRoles();
            s_instance = null;
        }

        private void UnloadAllRoles()
        {
            foreach (var role in allRoles)
            {
                AssetsManager.GetInstance().UnloadGameObject(role.gameObject);
            }
            allRoles.Clear();
        }

        private class Role
        {
            public string roleId = null;

            public Actor actor = null;

            public GameObject gameObject = null;
        }
    }
}

