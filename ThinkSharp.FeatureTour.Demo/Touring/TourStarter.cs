// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using ThinkSharp.FeatureTouring.Models;

namespace ThinkSharp.FeatureTouring.Touring
{
    internal static class TourStarter
    {
        public static void StartIntroduction()
        {
            var tour = new Tour
            {
                Name = "Introduction",
                ShowNextButtonDefault = true,
                Steps =
                [
                    new Step(ElementId.ButtonIntroduction, "Introduction", "Starts this tour."),
                    new Step(ElementId.ButtonActiveTour, "Active Tour", "Starts an active tour"),
                    new Step(ElementId.ButtonTourWithDialog, "Tour with Dialog", "Starts a tour that demonstrates how to created tours with dialogs."),
                    new Step(ElementId.ButtonPositioning, "Positioning", "Starts a tour that shows the popup positioning options"),
                    new Step(ElementId.ButtonCustomView, "CustomView", "Starts a tour that shows ho to define custom views"),
                    new Step(ElementId.ButtonOverView, "Tour Selection Menu", "Opens a menu screen that provides all available tours.")
                ]
            };
            tour.Start();
        }

        public static void StartPositioning()
        {
            var tour = new Tour
            {
                Name = "PositioningTour",
                ShowNextButtonDefault = true,
                Steps =
                [
                    new Step(ElementId.Rectangle, "Positioning", "TopLeft") {Tag = Placement.TopLeft},
                    new Step(ElementId.Rectangle, "Positioning", "TopCenter") {Tag = Placement.TopCenter},
                    new Step(ElementId.Rectangle, "Positioning", "TopRight") {Tag = Placement.TopRight},
                    new Step(ElementId.Rectangle, "Positioning", "RightTop") {Tag = Placement.RightTop},
                    new Step(ElementId.Rectangle, "Positioning", "RightCenter") {Tag = Placement.RightCenter},
                    new Step(ElementId.Rectangle, "Positioning", "RightBottom") {Tag = Placement.RightBottom},
                    new Step(ElementId.Rectangle, "Positioning", "BottomRight") {Tag = Placement.BottomRight},
                    new Step(ElementId.Rectangle, "Positioning", "BottomCenter") {Tag = Placement.BottomCenter},
                    new Step(ElementId.Rectangle, "Positioning", "BottomLeft") {Tag = Placement.BottomLeft},
                    new Step(ElementId.Rectangle, "Positioning", "LeftBottom") {Tag = Placement.LeftBottom},
                    new Step(ElementId.Rectangle, "Positioning", "LeftCenter") {Tag = Placement.LeftCenter},
                    new Step(ElementId.Rectangle, "Positioning", "LeftTop") {Tag = Placement.LeftTop},
                    new Step(ElementId.Rectangle, "Positioning", "Center") {Tag = Placement.Center}
                ]
            };
            tour.Start();
        }

        public static void StartActiveTour()
        {
            var tour = new Tour
            {
                Name = "Active Tour",
                ShowNextButtonDefault = false,
                Steps =
                [
                    new Step(ElementId.TextBoxResult, "Enter Calculation", "Enter the result of \"10 + 11\"."),
                    new Step(ElementId.TextBoxPath, "Select Path", "Enter path to the desktop."),
                    new Step(ElementId.ComboBoxOption, "Choose Option", "Choose \"OptionB\"."),
                    new Step(ElementId.ButtonClear, "Reset", "Press the button to reset the form and finish the tour.")
                ]
            };
            tour.Start();
        }

        public static void StartDialogTour()
        {
            var tour = new Tour
            {
                Name = "Dialog Tour",
                ShowNextButtonDefault = true,
                Steps =
                [
                    new Step(ElementId.ButtonPushMe, "Open Dialog", "Push the button to open a dialog and continue the tour. Note that the 'Next >>' button is disabled because the visual element that is associated with the next step is not yet available."),
                    new Step(ElementId.ButtonPushMeToo, "Dialog", "Push the button to continue") { ShowNextButton = false },
                    new Step(ElementId.ButtonClose, "Dialog", "Push the button to close the dialog."),
                    new Step(ElementId.ButtonPushMe, "Tour succeeded", "Tour finished.")
                ]
            };
            tour.Start();
        }

        public static void StartCustomViewTour()
        {
            var customizeHeaderViewModell = new CustomizeHeaderViewModel();
            var tour = new Tour
            {
                Name = "Custom View",
                ShowNextButtonDefault = true,
                Steps =
                [
                    new Step(ElementId.CustomView, "Image Content", null) { ContentDataTemplateKey = "ImageWithTextView" },
                    new Step(ElementId.CustomView, "Content With View Model", new BinaryCalculatorViewModel()) { ContentDataTemplateKey = "ViewWithViewModel" },
                    new Step(ElementId.CustomView, customizeHeaderViewModell, customizeHeaderViewModell)
                    {
                        HeaderDataTemplateKey = "CustomizeHeaderView",
                        ContentDataTemplateKey = "CustomizeHeaderContentView"
                    }
                ]
            };
            tour.Start();
        }

        public static void StartOverView()
        {
            var tour = new Tour
            {
                Name = "Overview",
                ShowNextButtonDefault = false,
                Steps =
                [
                    new Step(ElementId.CustomView, "Welcome - Select a tour", MainWindowViewModel.Instance)
                    {
                        ContentDataTemplateKey = "SelectTourDataTemplate"
                    }
                ]
            };
            tour.Start();
        }
    }
}
