-- 1. PrcResult���� ���������� ���а����� �ٸ� (����)�÷����� �и��Ͽ� ���� ���̺� ����
SELECT p.SchIdx , p.PrcDate, 
       CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOK,
	   CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
  FROM Process As p

-- 2. �հ� ���� ��� ���� ���� ���̺�
SELECT smr.SchIdx, smr.PrcDate, SUM(smr.PrcOK) AS PrcOkAmount, SUM(smr.PrcFail) AS PrcFailAmount
  FROM (
		SELECT p.SchIdx , p.PrcDate, 
          CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOK,
	      CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
          FROM Process As p
	   ) AS smr
 GROUP BY smr.SchIdx, smr.PrcDate

 -- 3.0 ���ι�
 SELECT * 
   FROM Schedules AS sch 
  INNER JOIN Process AS prc
     ON sch.SchIdx = prc.SchIdx

 -- 3.1 2�� ���(�������̺�)�� Schedules ���̺� �����ؼ� ���ϴ� ��� ����
 SELECT sch.SchIdx, sch.PlantCode, sch.SchAmount, prc.PrcDate, prc.PrcOkAmount, prc.PrcFailAmount
   FROM Schedules AS sch
  INNER JOIN (
				  SELECT smr.SchIdx, smr.PrcDate, SUM(smr.PrcOK) AS PrcOkAmount, SUM(smr.PrcFail) AS PrcFailAmount
				    FROM (
					    	SELECT p.SchIdx , p.PrcDate, 
						    CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOK,
						    CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
						    FROM Process As p
					     ) AS smr
				   GROUP BY smr.SchIdx, smr.PrcDate
  ) AS prc
    ON sch.SchIdx = prc.SchIdx
 WHERE sch.PlantCode = 'PC010002'
   AND prc.PrcDate BETWEEN '2021-06-29' AND '2021-06-30'