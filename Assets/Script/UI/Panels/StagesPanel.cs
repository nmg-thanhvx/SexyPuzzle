using UnityEngine;
using System.Collections;

public class StagesPanel : AnimatedPanel {

    [Header("\tPanel Field")]

    [SerializeField]
    UILabel panelNameLabel;
    [SerializeField]
    UIScrollView scrollView;

    #region Panel

    protected override void PanelWillShow()
    {
        scrollView.movement = UIScrollView.Movement.Vertical;
    }
    protected override void PanelDidShow()
    {

    }
    protected override void PanelWillHide()
    {

    }
    protected override void PanelDidHide()
    {

    }
    #endregion
}
