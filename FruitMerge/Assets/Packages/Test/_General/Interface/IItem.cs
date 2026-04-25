using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public interface IItemBase
    {
        public bool IsState<T>(T t) where T : System.Enum;
        public void ChangeState<T>(T t) where T : System.Enum;
        public void IfChangeState<T>(T i, T c) where T : System.Enum;
        public bool OnTake(IItemMoving item); //nhan lay gi do
        public void OnClickDown();
        public void OnDone();
        public bool IsDone { get; }
        public int OrderLayer { get; set; }
    }

    public interface IItemIdle : IItemBase
    {

    }    
    
    public interface IItemMoving: IItemBase
    {
        public Transform TF { get; }
        public bool IsCanMove { get; }
        public bool IsEnable { get; }
        public void SetOrder(int order);
        public void OnBack();
        public void OnClickTake();
        public void OnDrop();
        public void OnSavePoint();
        public void OnSave(float delayTime);
        public void OnSave(Vector3 savePoint);
        public void OnMove(Vector3 pos, Quaternion rot, float time);

    }
}
