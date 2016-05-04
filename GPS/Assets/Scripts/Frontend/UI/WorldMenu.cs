using CTD_Sim.Backend;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CTD_Sim
{
    namespace Frontend
    {
        public class WorldMenu : MonoBehaviour
        {
            public GameObject ratioPrefab;
            [Space()]
            public Text labelWorldScale;
            public Slider sliderWorldScale;
            public Text labelTimeScale;
            public Slider sliderTimeScale;
            public RectTransform carRatioParent;
            public Dropdown dropdownCarLabel;
            public Dropdown dropdownGraphType;

            private string formatWorldScale;
            private string formatTimeScale;

            void Start()
            {
                formatWorldScale = labelWorldScale.text;
                formatTimeScale = labelTimeScale.text;
                sliderWorldScale.value = World.WorldScale;
                sliderTimeScale.value = World.TimeScale;

                for (var i = 0; i < WorldFront.Instance.carPrefabs.Length; ++i)
                {
                    var ratio = Instantiate(ratioPrefab).transform;
                    ratio.SetParent(carRatioParent, false);
                    ratio.GetChild(0).GetComponent<Text>().text = WorldFront.Instance.carPrefabs[i].identifier.ToString();
                    ratio.GetChild(1).GetComponent<Slider>().onValueChanged.AddListener(GetRatioFunc(i));
                }

                dropdownCarLabel.options = Enum.GetNames(typeof(WorldFront.CarLabel)).Select(name => new Dropdown.OptionData(name)).ToList();
                dropdownGraphType.options = Enum.GetNames(typeof(WorldFront.CarGraphType)).Select(name => new Dropdown.OptionData(name)).ToList();
            }

            void Update()
            {
                labelTimeScale.text = string.Format(formatTimeScale, World.TimeScale);
            }

            public void OnWorldScaleChange()
            {
                World.WorldScale = sliderWorldScale.value;
                labelWorldScale.text = string.Format(formatWorldScale, World.WorldScale);
            }

            public void OnTimeScaleChange()
            {
                World.TimeScale = sliderTimeScale.value;
                labelTimeScale.text = string.Format(formatTimeScale, World.TimeScale);
            }

            private UnityEngine.Events.UnityAction<float> GetRatioFunc(int id)
            {
                return (val) => WorldFront.Instance.carRatio[id] = val;
            }

            public void OnCarLabelChange()
            {
                WorldFront.Instance.carLabel = (WorldFront.CarLabel)dropdownCarLabel.value;
            }

            public void OnGraphTypeChange()
            {
                WorldFront.Instance.graphType = (WorldFront.CarGraphType)dropdownGraphType.value;
            }
        }
    }
}