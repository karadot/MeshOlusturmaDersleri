using UnityEngine;

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter))]
public class AyrikUcgenler : MonoBehaviour {
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    /*
    Editor üzerinde farkı görebilmemiz için 
    tek bir vertexi Y ekseninde manipülede kullanacağımız değişken
    */
    [SerializeField]
    float yukseklik;
    //Yaratılacak Mesh tipini seçmemizi sağlayan değişken
    [SerializeField]
    MeshTipi meshTipi;

    /*
    Editorde scriptimiz üzerinde bir değişiklik yaptığımızda
    OnValidate sayesinde seçtiğimiz Mesh tipine göre
    güncelleyebileceğiz
    */
    void OnValidate () {
        mesh = GetComponent<MeshFilter> ().mesh;

        if (meshTipi == MeshTipi.Devamli) {
            DevamliMeshVerisiOlustur ();
        } else {
            AyrikMeshVerisiOlustur ();
        }
        MeshOlustur ();
    }

    /*
    Ayrık Mesh verisi oluştururken öncekinden(Devamli) farklı olarak
    4 yerine 6 vertex kullanıyoruz. 
    Burada amaç 1 kare şeklini 2 üçgen ile oluştururken
    üçgenlerin ortak vertex kullanmaması.
     */
    void AyrikMeshVerisiOlustur () {
        /*
        1+3---4
        |     |
        |     |
        0-----2+5
        Aşağıda verdiğimiz konumları görsel olarak 
        kafanızda canlandırabilmeniz için 
        */
        /*
        Sırasıyla her bir köşeye vertexleri yerleştiriyoruz
        Dikkat ederseniz aynı konumu paylaşan 
        1-3 ve 2-5 indexlerine sahip vertexler var.
        Aynı konumu paylaşsalar da bağımsız vertexler
        */
        vertices = new Vector3[] {
            new Vector3 (0, yukseklik, 0), //0,0,0
            Vector3.forward, //0,0,1
            Vector3.right, //1,0,0
            Vector3.forward, //0,0,1
            Vector3.right + Vector3.forward, //1,0,1
            Vector3.right, //1,0,0
        };

        /*
        0. üçgen : 0 1 2
        1. üçgen : 3 4 5
        */
        /*
        Oluşturduğumuz vertexlerin index değerlerini triangles dizimize
        aktararak üçgenleri oluşturuyoruz.
        */
        triangles = new int[] {
            0, 1, 2, //0.üçgen
            3, 4, 5  // 1.üçgen
        };
    }

    void DevamliMeshVerisiOlustur () {
        /*
        Devamlı mesh yapısında bir kareyi oluştururken
        yarattığımız 2 üçgen ortak vertexler kullanır.
        1 karede de 4 köşeden/noktadan yarattığımızı düşünürsek
        4 adet vertex olması mantıklıdır.
        */
        vertices = new Vector3[] {
            new Vector3 (0, yukseklik, 0), //0,0,0
            Vector3.forward, //0,0,1
            Vector3.right, //1,0,0
            Vector3.right + Vector3.forward //1,0,1
        };
        /*
        1-----3
        |     |
        |     |
        0-----2
        yaratacağımız karenin vertexlerinin zihninizde canlanması için 
        bu yapıyı oluşturdum.
        */
        triangles = new int[] {
            0,1,2, //0.üçgen 
            1,3,2  //1.üçgen
        };
    }

    /*
    Burada yaptığımız şey sırasıyla şöyle
    Mesh verisini temizliyoruz.
    Mesh'i oluşturacak vertexlerin atamasını yapıyoruz.
    Mesh'i oluşturacak üçgenlerin atamasını yapıyoruz.
    Üçgenlerin Normal değerlerini hesaplatıyoruz.
    */
    public void MeshOlustur () {
        mesh.Clear ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();
    }
}
public enum MeshTipi {
    Devamli,
    Ayrik
}
