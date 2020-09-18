using UnityEngine;

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter))]
public class TemelDers : MonoBehaviour {

    /*
    Mesh aslında bir sürü noktanın birbirine bağlanması ile oluşur.
    Bu noktalara vertex denir, çoğul olarak da vertices.
    Bu noktalar aslında bir konum değerini belirtir.
    Bu yüzden Vector3 tipi değişkenlerde tutmak mantıklıdır.
    Birden fazla vertex kullanmamız gerektiği için, bunları bir dizide tutacağız.
    */
    Vector3[] vertices;
    /*
    Meshler yalnızca vertexlere sahip olarak oluşturabileceğimiz yapılar değil.
    Bunların birbirine bağlanması gerekiyor. Bu bağlama işlemini de aslında
    üçgenler yaratarak oluşturuyoruz. 
    Triangles dizisinde bu üçgenlerin verisini tutacağız.
    */
    int[] triangles;

    Mesh mesh;

    void Start () {
        /*
        Varolan MeshFilter bileşeninin mesh değerini bizim değişkenimize atıyoruz.
        Bu sayede mesh değişkenimizi güncellediğimizde, bu bileşen de güncellenecek
        biz de oluşturduğumuz mesh yapısını görebileceğiz
        */
        mesh = GetComponent<MeshFilter> ().mesh;

        DikdortgenMeshVerisi ();
        MeshOlustur ();

    }

    /*
    Mesh yapımızı oluşturmak için gereken vertex
    ve üçgenleri oluşturuyoruz
    */

    void DikdortgenMeshVerisi () {
        /*
       Basit bir dikdörtgen oluşturmak için yapmamız gereken şey 
       2 adet üçgeni birleştirerek bunu yapmak. Bir dikdörtgenin 4 tane 
       ayrı noktadan 
        */
        vertices = new Vector3[] {
            Vector3.zero, //0,0,0
            Vector3.forward, //0,0,1
            Vector3.right, //1,0,0
            Vector3.right + Vector3.forward //1,0,1
        };
        /*
        1-----3
        | \   |
        |   \ |
        0-----2
        yaratacağımız karenin vertexlerinin zihninizde canlanması için 
        bu yapıyı oluşturdum.
        Bu aşamada artık üçgenleri oluşturmamız gerekiyor. Bunun için
        yapmamız gereken de üçgeni oluşturacak noktaları triangles dizisine sırayla eklemek olacak.
        Dikkat etmemiz gereken bir kural var, o da verdiğimiz vertexlerin 
        görünmesini istediğimiz yöne göresaat yönünde sıralamaya sahip olmaları.
        Triangle içerisine eklediğimiz her 3 değer, 1 üçgeni temsil etmekte. 
        Aşağıdaki yazım şekliyle bunu anlatabildiğimi umuyorum.
        */
        triangles = new int[] {
            0, 1, 2, //0.üçgen 
            1, 3, 2  //1.üçgen
        };

        //Aşağıdaki kodu yorum satırı halinden çıkarırsanız, 
        //bir üçgenin ters yönde render edildiğini, gösterildiğini göreceksiniz.
        /*
        triangles = new int[] {
            0, 1, 2, //0.üçgen 
            1, 2, 3  //1.üçgen
        };
        */
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
