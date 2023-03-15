using UnityEngine;

public class BubblePatch : BaseObject
{
    private int AlterableValueA;
    private int AlterableValueB;

    private void FixedUpdate()
    {
        if (XPosition < SceneController.XRightFrame + 256f &&
            XPosition > SceneController.XLeftFrame - 256f &&
            YPosition > SceneController.YBottomFrame - 32f &&
            YPosition < SceneController.YTopFrame + 32f)
        {
            if ((LevelController.GlobalTimer % Random.Range(30, 60)) == 0)
            {
                Bubble small = SceneController.CreateStageObject("Bubble", XPosition, YPosition) as Bubble;
                small.BubbleSize = Bubble.Bubble_Sizes.Small;
                small.XPosition = XPosition;
                small.AlterableValueB = Random.Range(0, 200);
            }

            if ((LevelController.GlobalTimer % Random.Range(90, 120)) == 0)
            {
                AlterableValueA = Random.Range(0, 3);
                AlterableValueB = 1;
            }

            if (AlterableValueB == 1 && AlterableValueA != 0)
            {
                AlterableValueB = 0;
                Bubble medium = SceneController.CreateStageObject("Bubble", XPosition, YPosition) as Bubble;
                medium.BubbleSize = Bubble.Bubble_Sizes.Medium;
                medium.XPosition = XPosition;
                medium.AlterableValueB = Random.Range(0, 200);
            }
            if (AlterableValueB == 1 && AlterableValueA == 0)
            {
                AlterableValueB = 0;
                Bubble large = SceneController.CreateStageObject("Bubble", XPosition, YPosition) as Bubble;
                large.BubbleSize = Bubble.Bubble_Sizes.Large;
                large.XPosition = XPosition;
                large.AlterableValueB = Random.Range(0, 200);
            }
        }
    }
}
