using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StudyMaterial{
    YellowPaper,
    BluePaper,
    WorkSheet,
    Book,
    Bag,
    PencilBox,
    NoteBook,
    Tablet,
    MilkBottle,
    Pencil,
    Pen,
    Ruler,
    OpenBook,
    LunchBox,
    Eraser,
    Glue,
    Scissor,
    RippedPaper,
    Apple,
    BoxFilled,
    WeeklyPlan
}

public class StudyMaterialType : MonoBehaviour
{
    

    public GameObject YellowPaper, BluePaper,WorkSheet, NoteBook,Tablet, Book, OpenBook, PencilBox, Pencil, Pen, LunchBox, MilkBottle, Bag, Ruler, Eraser, Glue, Scissor, RippedPaper, Apple , BoxFilled, WeeklyPlan;

    public void HideByDefaultStudyMaterials()
    {
        if(BoxFilled != null)
        {
            BoxFilled.SetActive(false);
        }
        if (YellowPaper!=null)
        {
           YellowPaper.SetActive(false);
        }
        if (BluePaper != null)
        {
            BluePaper.SetActive(false);
        }
        if (WorkSheet != null)
        {
            WorkSheet.SetActive(false);
        }
        if (NoteBook != null)
        {
            NoteBook.SetActive(false);
        }
        if (Tablet != null)
        {
            Tablet.SetActive(false);
        }
        if (Book != null)
        {
            Book.SetActive(false);
        }
        if (OpenBook != null)
        {
            OpenBook.SetActive(false);
        }
        if (PencilBox != null)
        {
            PencilBox.SetActive(false);
        }
        if (Pencil != null)
        {
            Pencil.SetActive(false);
        }
        if (Pen != null)
        {
            Pen.SetActive(false);
        }
        if (LunchBox != null)
        {
            LunchBox.SetActive(false);
        }
        if (MilkBottle != null)
        {
            MilkBottle.SetActive(false);
        }
        if (Bag != null)
        {
            Bag.SetActive(false);
        }
        if (Ruler != null)
        {
            Ruler.SetActive(false);
        }
        if (Eraser != null)
        {
            Eraser.SetActive(false);
        }
        if (Glue != null)
        {
            Glue.SetActive(false);
        }
        if (Scissor != null)
        {
            Scissor.SetActive(false);
        }
        if (RippedPaper != null)
        {
            RippedPaper.SetActive(false);
        }
        if (Apple != null)
        {
            Apple.SetActive(false);
        }
       
        if (WeeklyPlan != null)
        {
            WeeklyPlan.SetActive(false);
        }

    }

    public GameObject GetStudyMaterial(StudyMaterial studyMaterial) {

        switch (studyMaterial)
        {
            case StudyMaterial.BoxFilled:
                return BoxFilled;
            case StudyMaterial.YellowPaper:
                return YellowPaper;
            case StudyMaterial.BluePaper:
                return BluePaper;
            case StudyMaterial.WorkSheet:
                return WorkSheet;
            case StudyMaterial.Book:
                return Book;
            case StudyMaterial.Tablet:
                return Tablet;
            case StudyMaterial.NoteBook:
                return NoteBook;
            case StudyMaterial.OpenBook:
                return OpenBook;
            case StudyMaterial.PencilBox:
                return PencilBox;
            case StudyMaterial.Pencil:
                return Pencil;
            case StudyMaterial.Pen:
                return Pen;
            case StudyMaterial.LunchBox:
                return LunchBox;
            case StudyMaterial.MilkBottle:
                return MilkBottle;
            case StudyMaterial.Bag:
                return Bag;
            case StudyMaterial.Ruler:
                return Ruler;
            case StudyMaterial.Eraser:
                return Eraser;
            case StudyMaterial.Glue:
                return Glue;
            case StudyMaterial.Scissor:
                return Scissor;
            case StudyMaterial.RippedPaper:
                return RippedPaper;
            case StudyMaterial.Apple:
                return Apple;
            case StudyMaterial.WeeklyPlan:
                return WeeklyPlan;
            default:
                return BluePaper;

        }
    }

    public void SetStudyMaterialVisiblity(StudyMaterial studyMaterial,bool visibleOrNot)
    {

        switch (studyMaterial)
        {
            case StudyMaterial.BoxFilled:
                BoxFilled.SetActive(visibleOrNot);
                break;
            case StudyMaterial.YellowPaper:
                YellowPaper.SetActive(visibleOrNot);
                break;
            case StudyMaterial.BluePaper:
                BluePaper.SetActive(visibleOrNot);
                break;
            case StudyMaterial.WorkSheet:
                WorkSheet.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Book:
                Book.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Tablet:
                Tablet.SetActive(visibleOrNot);
                break;
            case StudyMaterial.NoteBook:
                NoteBook.SetActive(visibleOrNot);
                break;
            case StudyMaterial.OpenBook:
                OpenBook.SetActive(visibleOrNot);
                break;
            case StudyMaterial.PencilBox:
                PencilBox.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Pencil:
                Pencil.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Pen:
                Pen.SetActive(visibleOrNot);
                break;
            case StudyMaterial.LunchBox:
                LunchBox.SetActive(visibleOrNot);
                break;
            case StudyMaterial.MilkBottle:
                MilkBottle.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Bag:
                Bag.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Ruler:
                Ruler.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Eraser:
                Eraser.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Glue:
                Glue.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Scissor:
                Scissor.SetActive(visibleOrNot);
                break;
            case StudyMaterial.RippedPaper:
                RippedPaper.SetActive(visibleOrNot);
                break;
            case StudyMaterial.Apple:
                Apple.SetActive(visibleOrNot);
                break;
            case StudyMaterial.WeeklyPlan:
                WeeklyPlan.SetActive(visibleOrNot);
                break;
            default:
                BluePaper.SetActive(visibleOrNot);
                break;

        }
    }


    public bool SetStudyMaterialVisiblityAndRetrun(StudyMaterial studyMaterial, bool visibleOrNot)
    {

        switch (studyMaterial)
        {
            case StudyMaterial.BoxFilled:

                if (BoxFilled != null) { BoxFilled.SetActive(visibleOrNot); } else return false;
                break;
            case StudyMaterial.YellowPaper:
                if (YellowPaper != null) { YellowPaper.SetActive(visibleOrNot); } else return false;
                break;
            case StudyMaterial.BluePaper:
                if (BluePaper != null)
                {
                    BluePaper.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.WorkSheet:
                if (WorkSheet != null)
                {
                    WorkSheet.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Book:
                if (Book != null)
                {
                    Book.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Tablet:
                if (Tablet != null)
                {
                    Tablet.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.NoteBook:
                if (NoteBook != null)
                {
                    NoteBook.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.OpenBook:
                if (OpenBook != null)
                {
                    OpenBook.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.PencilBox:
                if (PencilBox != null)
                {
                    PencilBox.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Pencil:
                if (Pencil != null)
                {
                    Pencil.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Pen:
                if (Pen != null)
                {
                    Pen.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.LunchBox:
                if (LunchBox != null)
                {
                    LunchBox.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.MilkBottle:
                if (MilkBottle != null)
                {
                    MilkBottle.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Bag:
                if (Bag != null)
                {
                    Bag.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Ruler:
                if (Ruler != null)
                {
                    Ruler.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Eraser:
                if (Eraser != null)
                {
                    Eraser.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Glue:
                if (Glue != null)
                {
                    Glue.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.Scissor:
                if (Scissor != null)
                {
                    Scissor.SetActive(visibleOrNot);
                }
                else return false;
                break;
            case StudyMaterial.RippedPaper:
                if (RippedPaper != null)
                {
                    RippedPaper.SetActive(visibleOrNot);
                } else return false;
        break;
            case StudyMaterial.Apple:
                if (Apple != null)
                {
                    Apple.SetActive(visibleOrNot);
                }
                else return false;
                break;
            default:
               if (BluePaper != null)
                    {
                        BluePaper.SetActive(visibleOrNot);
                }
                else return false;
                break;

        }
        return true;
    }

    public void RandomlyShowOrHideStudyMaterialsOnDesk()
    {

        int yesOrNo;

        if (BoxFilled != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) BoxFilled.SetActive(true); else BoxFilled.SetActive(false);
        }
        if (NoteBook != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) NoteBook.SetActive(true); else NoteBook.SetActive(false);
        }
        if (Tablet != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Tablet.SetActive(true); else Tablet.SetActive(false);
        }
        if (Book != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Book.SetActive(true); else Book.SetActive(false);
        }
        if (PencilBox != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) PencilBox.SetActive(true); else PencilBox.SetActive(false);
        }
        if (Pencil != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Pencil.SetActive(true); else Pencil.SetActive(false);
        }
        if (Pen != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Pen.SetActive(true); else Pen.SetActive(false);
        }
        if (LunchBox != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) LunchBox.SetActive(true); else LunchBox.SetActive(false);
        }
        if (MilkBottle != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) MilkBottle.SetActive(true); else MilkBottle.SetActive(false);
        }
        if (Ruler != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Ruler.SetActive(true); else Ruler.SetActive(false);
        }
        if (Eraser != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Eraser.SetActive(true); else Eraser.SetActive(false);
        }
        if (Glue != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Glue.SetActive(true); else Glue.SetActive(false);
        }
        if (Scissor != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Scissor.SetActive(true); else Scissor.SetActive(false);
        }
        if (Apple != null)
        {
            yesOrNo = Random.Range(0, 11) % 2;
            if (yesOrNo == 1) Apple.SetActive(true); else Apple.SetActive(false);
        }

    }
}
