using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerInfoComparer : IComparer<NetPlayerInfo>
{
    public int Compare(NetPlayerInfo x, NetPlayerInfo y)
    {
        // ���� ��
        if(y.isFinish.Value == x.isFinish.Value)
        {
            // �� �� ���ָ� ���ߴٸ�
            if (y.isFinish.Value == 0)
            {
                //�������� �������� �켱
                int lapComparison = y.Lap.Value.CompareTo(x.Lap.Value);
                if (lapComparison != 0) return -lapComparison;

                //���� ������ ���� ����� ���� �켱
                int rpNumComparison = y.RpNum.Value.CompareTo(x.RpNum.Value);
                if (rpNumComparison != 0) return -rpNumComparison;

                //���� ���������̶�� �󸶳� �� �ָ����ִ���(������)
                return -y.CheckPointDistance.Value.CompareTo(x.CheckPointDistance.Value);
            }
            else
            {
                int timeComparison = y.LapTimes.Value.CompareTo(x.LapTimes.Value);
                if (timeComparison != 0) return timeComparison;
            }
        }
        else
        {
            //���� �� ���� �켱 ����
            return x.isFinish.Value > y.isFinish.Value ? 1 : -1;
        }

        return 0;
        

    }
}
