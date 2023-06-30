using System.Collections.Generic;
using System.IO;
using Extensions;
using Newtonsoft.Json;
using Services;
using UniRx;
using UnityEngine;

namespace Profiles
{
    public class ProfileService : IService
    {
        private static readonly string Directory = Path.Combine(Application.persistentDataPath, "Profiles");
        private static readonly List<Profile> _profiles = new();

        public bool Initialized { get; set; }
        
        public void Initialize()
        {
            Observable.EveryApplicationFocus().Subscribe(OnFocus);
            Observable.EveryApplicationPause().Subscribe(OnPause);
            Initialized = true;
        }

        public static T LoadService<T>() where T : Profile
        {
            var file = Directory + "/" + typeof(T) + ".json";
            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            if (!File.Exists(file))
            {
                File.WriteAllText(file, new Profile().ToJson());
            }

            JsonReader reader = new JsonTextReader(File.OpenText(file));
            JsonSerializer serializer = new JsonSerializer();
            var profile = serializer.Deserialize<T>(reader);
            _profiles.Add(profile);
            return profile;
        }

        public void SaveAll()
        {
            foreach (Profile profile in _profiles)
            {
                Save(profile);
            }
        }

        private void OnFocus(bool hasFocus)
        {
            if(hasFocus)
                return;
            
            SaveAll();
        }
        private void OnPause(bool hasFocus)
        {
            if(!hasFocus)
                return;

            SaveAll();
        }

        private void Save<T>(T profile) where T : Profile
        {
            Debug.Log("SAVE PROFILE " + profile.GetType());
            File.WriteAllText(Directory + "/" + profile.GetType() + ".json", profile.GetJson());
        }
    }
}