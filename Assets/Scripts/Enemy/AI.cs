using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    //private Enemy_Attack _attack;

    public Animator animator;
    public AudioSource audioSource;

    public Transform player;

    //扇形範圍區域探查距離
    public float probeDistance;
    //扇形範圍區域探查方向
    public float probeAngle;
    float distance;
    SimpleCharacterControl pc;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        pc = player.GetComponent<SimpleCharacterControl>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        if (distance < probeDistance)
        {
            if (CheackScope(transform.position, player.position))
            {
                if (pc.m_State.IsName("Base Layer.Climb.Climbing") ||pc.m_State.IsName("Base Layer.Climb.ClimbUp") || pc.m_State.IsName("Base Layer.Climb.ClimbDown"))
                { }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        Debug.Log("進入檢測範圍");
                        audioSource.Play();
                        pc.LockPlayerControl();
                        StartCoroutine("Timer");
                    }

                }
            }
            else
            {
                Debug.Log("沒進入檢測範圍");
            }
        }
        else
        {
        }
    }

    private bool CheackScope(Vector3 _avatarPos, Vector3 _enemyPos)
    {
        Vector3 srcVect = _enemyPos - _avatarPos;
        Vector3 fowardPos = transform.forward * 1 + _avatarPos;
        Vector3 fowardVect = fowardPos - _avatarPos;
        fowardVect.y = 0;
        float angle = Vector3.Angle(srcVect, fowardVect);
        if (angle < probeAngle / 2)
        { return true; }
        else
        { return false; }
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        pc.Dead(); 
    }

    private float innerRadius = 3 ;

    public int Segments = 60;//分割数  

    //畫出扇形
    // private MeshFilter meshFilter;
    // private void OnDrawGizmos()
    // {
    //     meshFilter = GetComponent<MeshFilter>();
    //     meshFilter.mesh = CreateMesh(probeDistance, innerRadius, probeAngle, Segments);
    // }

    // Mesh CreateMesh(float radius, float innerradius,float angledegree,int segments)
    // {
    //     //vertices(顶点):
    //     int vertices_count = segments* 2+2;              //因为vertices(顶点)的个数与triangles（索引三角形顶点数）必须匹配
    //     Vector3[] vertices = new Vector3[vertices_count];       
    //     float angleRad = Mathf.Deg2Rad * angledegree;
    //     float angleCur = angleRad;
    //     float angledelta = angleRad / segments;
    //     for(int i=0;i< vertices_count; i+=2)
    //     {
    //         float cosA = Mathf.Cos(angleCur);
    //         float sinA = Mathf.Sin(angleCur);
    //         vertices[i] = new Vector3(radius * cosA, 0, radius * sinA);
    //         vertices[i + 1] = new Vector3(innerradius * cosA, 0, innerradius * sinA);
    //         angleCur -= angledelta;
    //     }
    //     //triangles:
    //     int triangle_count = segments * 6;
    //     int[] triangles = new int[triangle_count];
    //     for(int i=0,vi=0;i<triangle_count;i+=6,vi+=2)   
    //     {
    //         triangles[i] = vi;
    //         triangles[i + 1] = vi+3;
    //         triangles[i + 2] = vi + 1;
    //         triangles[i + 3] =vi+2;
    //         triangles[i + 4] =vi+3;
    //         triangles[i + 5] =vi;
    //     }
    //     //uv:
    //     Vector2[] uvs = new Vector2[vertices_count];
    //     for (int i = 0; i < vertices_count; i++)
    //     {
    //         uvs[i] = new Vector2(vertices[i].x / radius / 2 + 0.5f, vertices[i].z / radius / 2 + 0.5f);
    //     }
    //     //负载属性与mesh
    //     Mesh mesh = new Mesh();
    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.uv = uvs;
    //     return mesh;
    // }
}
