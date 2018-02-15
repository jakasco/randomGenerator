using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenerateHouse : MonoBehaviour
{

    public GameObject floor2;
    public GameObject wall;
    public float doorWide = 3f;
    public GameObject door;
    public float sizeF = 20f;
    private Vector3 temp;
    private bool[] randomList;
    private int[] suunnat = new int[4];
    private GameObject[][] wallList = new GameObject[4][];
    private float[][] roomList;
    // Use this for initialization
    void Start()
    {
        buildMultiple(0, 0);
    }

    void createOne() {
        buildMultiple(0, 0);
    }

    void createAll()
    {

        buildMultiple(0, 0);

        //Jos tehdään isompi kenttä
        
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (gameObj.name == "Center")  //etsitään kaikki keskikohdat josta haarautuu käytävät
            {
                float x = gameObj.transform.position.x;
                float z = gameObj.transform.position.z;
                buildMultiple(x, z);
            }
        }     
    }

    void Update()
    {

        //uudelleen generoidaan napilla
        if (Input.GetKeyDown(KeyCode.M))
        {
            deleteAll();
            createAll();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            deleteAll();
            createOne();
        }

    }

    void deleteAll()
    {
        GameObject[] a = GameObject.FindGameObjectsWithTag("a");
        for (int i = 0; i < a.Length; i++)
        {
            Destroy(a[i]);
        }
    }


    void buildMultiple(float x, float y)
    {
        //Luodaan ensimmäinen, ja siitä eteenpäin jatketaan reitteijä riippuen booleanista
        float[] f = buildRoom(x, y, false, false, false, false);

        if (f[0] > 0)
        { 
            float x1 = x;
            float x2 = y - (3*sizeF);
            buildRoom(x1, x2, false, true, false, false);
        }
        if (f[1] > 0)
        {
            float x1 = x;
            float x2 = y + (3 * sizeF);
            buildRoom(x1, x2, true, false, false, false);          
        }
        if (f[2] > 0)
        {
            float x1 = x;
            float x2 = y - (3 * sizeF);
            buildRoom(x2, x1, false, false, false, true);   
        }
        if (f[3] > 0)
        {
            float x1 = x;
            float x2 = y + (3 * sizeF);
            buildRoom(x2, x1, false, false, true, false);       
        }
    }

    //tehdään huone, jossa on seinät generoitu randomisti jatkamaan reittejeä
    float[] buildRoom(float x, float z, bool s, bool n, bool w, bool e)
    {
        GameObject f = createNewFloor(x, 0, z, sizeF, sizeF);

        //annetaan keskiosalle tunniste, jota käytetään generoidessa enemmän huoneita
        f.gameObject.name = "Center";
        randomList = randomoi(s, n, w, e);

        //2d array joka seinälle, [0] = 1 seinä, myöhemmin jos ovi tulee, niin [1] on seinän toinen puoli
        wallList[0] = new GameObject[2];
        wallList[1] = new GameObject[2];
        wallList[2] = new GameObject[2];
        wallList[3] = new GameObject[2];
        float[] a = buildWalls(f, randomList);

        //palautetaan listana ilmansuunnat, mihin reittejä jatketaan
        return a;
    }

    //Anna seinille 1/2 random luku
    bool[] randomoi(bool s, bool n, bool w, bool e)
    {
        bool[] x = new bool[4];
        bool[] directions = new bool[] { s, n, w, e };
        int ra = 0;
        for (int i = 0; i < 4; i++)
        {
            ra = Random.Range(0, 2);  // 2 tai 1
            if (ra == 0)
            {
                x[i] = true;
            }
            else
            {
                x[i] = false;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (directions[i] == true)
            {  //Varmistetaan  huoneiden yhdistyminen
                x[i] = true;
            }
        }
        return x;
    }


    //lattian x-koon skaalaus ja liikuttamine alkuperäiseen kohtaan
    void plusXScale(GameObject floor, float p)
    {
        temp = floor.transform.localScale;
        temp.x += p;
        floor.transform.localScale = temp;
        temp = floor.transform.position;
        temp.x += (floor.transform.localScale.x / 2);
        floor.transform.position = temp;
    }

    //lattian z-koon skaalaus ja liikuttamine alkuperäiseen kohtaan
    void plusZScale(GameObject floor, float p, bool etuTaka)
    {
        temp = floor.transform.localScale;
        temp.z += p;
        floor.transform.localScale = temp;
        temp = floor.transform.position;
        if (etuTaka == true)
        {
            temp.z -= (floor.transform.localScale.z / 4);
        }
        else
        {
            temp.z += (floor.transform.localScale.z / 4);
        }
        floor.transform.position = temp;
    }

    void editFloorLenght(GameObject floor, GameObject wall, float suunta, bool etuTaka)
    {

        float p = 15f;
        if (suunta == 0)
        {
            plusXScale(floor, p);
        }
        else if (suunta == 1)
        {
            plusXScale(floor, p);
        }
        else if (suunta == 2)
        {
            plusZScale(floor, p, etuTaka);
        }
        else if (suunta == 3)
        {
            plusZScale(floor, p, etuTaka);
        }

    }


    //Seinien luonti metodi, missä on luodaan seinät sekä mahdolliset ovet
    float[] buildWalls(GameObject floor, bool[] raList)
    {
        float[] nextWall = new float[] { 0, 0, 0, 0 };

        float x = floor.transform.position.x;  //lattian sijainti
        float z = floor.transform.position.z;
        float y = floor.transform.position.y;
        float xScale = 0f;  //Lattian leveys pituus yms
        float yScale = 0f;
        float zScale = 0f;

        //Luodaan kaikki neljä seinää
        wallPosition(floor, x, y, z, xScale, yScale, zScale, true, raList[0]);
        wallPosition(floor, x, y, z, xScale, yScale, zScale, false, raList[1]);
        wallPosition2(floor, x, y, z, xScale, yScale, zScale, true, raList[2]);
        wallPosition2(floor, x, y, z, xScale, yScale, zScale, false, raList[3]);

        //Jos tähän suuntaan on huone, niin scaalataan vastakkaisia seiniä
        Vector2 startingPoint = new Vector2(floor.transform.position.x, floor.transform.position.z);

        //käydään  läpi kaikki ilmansuunnat
        if (raList[0] == true)
        {
            createNewFloor(startingPoint.x, 0, startingPoint.y - sizeF, sizeF, sizeF); //luodaan uusi lattiapala alaspäin
            nextWall[0] = sizeF;
            if (raList[3] == true)
            {
                minusZ(wallList[3][1], sizeF);
            }
            else
            {
                minusZ(wallList[3][0], sizeF);
            }
            if (raList[2] == true)
            {  
                minusZ(wallList[2][0], sizeF);
            }
            else
            {
                minusZ(wallList[2][0], sizeF);
            }
        }

        if (raList[1] == true)
        {
            nextWall[1] = sizeF;
            createNewFloor(startingPoint.x, 0, startingPoint.y + sizeF, sizeF, sizeF);
            if (raList[3] == true)
            {
                plusZ(wallList[3][0], sizeF);
            }
            else
            {
                plusZ(wallList[3][0], sizeF);
            }
            if (raList[2] == true)
            {
                plusZ(wallList[2][1], sizeF);
            }
            else
            {
                plusZ(wallList[2][0], sizeF);
            }
        }

        if (raList[2] == true)
        {
            nextWall[2] = sizeF;
            createNewFloor(startingPoint.x - sizeF, 0, startingPoint.y, sizeF, sizeF);
            if (raList[0] == true)
            {
                minusX(wallList[0][0], sizeF);
            }
            else
            {
                minusX(wallList[0][0], sizeF);
            }

            if (raList[1] == true)
            {
                minusX(wallList[1][1], sizeF);
            }
            else
            {
                minusX(wallList[1][0], sizeF);
            }
        }

        if (raList[3] == true)
        {
            nextWall[3] = sizeF;
            createNewFloor(startingPoint.x + sizeF, 0, startingPoint.y, sizeF, sizeF);

            if (raList[0] == true)
            {
                plusX(wallList[0][1], sizeF);
            }
            else
            {
                plusX(wallList[0][0], sizeF);
            }

            if (raList[1] == true)
            {
                plusX(wallList[1][0], sizeF);
            }
            else
            {
                plusX(wallList[1][0], sizeF);
            }

        }
        return nextWall;
    }

    //Uuden lattia palasen luominen
    GameObject createNewFloor(float x, float y, float z, float xSize, float zSize)
    {
        GameObject floor = Instantiate(floor2, new Vector3(x, y, z), Quaternion.identity);
        floor.tag = "a";
        temp = new Vector3(xSize, floor.transform.localScale.y, zSize);
        floor.transform.localScale = temp;
        return floor;
    }

    //Seinien skaalaus ja liikutus eri suuntiin
    void minusZ(GameObject go, float amount)
    {
        temp = go.transform.localScale;
        temp.x += amount;
        go.transform.localScale = temp;
        temp = go.transform.position;
        temp.z -= amount / 2;
        go.transform.position = temp;
    }

    void plusZ(GameObject go, float amount)
    {
        temp = go.transform.localScale;
        temp.x += amount;
        go.transform.localScale = temp;
        temp = go.transform.position;
        temp.z += amount / 2;
        go.transform.position = temp;
    }

    void minusX(GameObject go, float amount)
    {
        temp = go.transform.localScale;
        temp.x += amount;
        go.transform.localScale = temp;
        temp = go.transform.position;
        temp.x -= amount / 2;
        go.transform.position = temp;
    }

    void plusX(GameObject go, float amount)
    {
        temp = go.transform.localScale;
        temp.x += amount;
        go.transform.localScale = temp;
        temp = go.transform.position;
        temp.x += amount / 2;
        go.transform.position = temp;
    }


    //Jokainen seinä asetellaan ja skaalataan lattian mukaan
    void wallPosition(GameObject floor, float x, float y, float z, float xScale, float yScale, float zScale, bool takaEtu, bool raBool)
    {
        xScale = floor.transform.localScale.x;
        zScale = floor.transform.localScale.z;
        float yScale2 = floor.transform.localScale.y;
        //Jaetaan seinät toisille puolilleen
        if (takaEtu == true)
        {
            x = x - (xScale / 2);
            z = z - (zScale / 2);
        }
        else
        {
            x = x + (xScale / 2);
            z = z + (zScale / 2);
        }
        GameObject newWall = Instantiate(wall, new Vector3(x, y, z), Quaternion.identity);
        newWall.tag = "a";
        //muutetaan seinän pituus lattian pituudeksi
        temp = newWall.gameObject.transform.localScale;
        temp.x = xScale;
        newWall.gameObject.transform.localScale = temp;

        //Seinät alkavat lattian korkeudesta
        yScale = newWall.transform.localScale.y;
        y = y + (yScale / 2) - (yScale2 / 2);

        //Sijoitetaan oikeaan kohtaan
        temp = newWall.gameObject.transform.position;
        if (takaEtu == true)
        {
            wallList[0][0] = newWall;
            temp.x += newWall.gameObject.transform.localScale.x / 2;
            newWall.gameObject.name = "South";
        }
        else
        {
            wallList[1][0] = newWall;
            temp.x -= newWall.gameObject.transform.localScale.x / 2;
            newWall.gameObject.name = "North";
        }
        temp.y = y;
        newWall.gameObject.transform.position = temp;
        //Jos on ovi aukko huoneessa, tehdään ovi
        if (raBool == true)
        {
            int i = CreateDoor(floor, newWall, x, z, y, takaEtu);
        }
    }

    //vaaka suunnassa olevat seinät
    void wallPosition2(GameObject floor, float x, float y, float z, float xScale, float yScale, float zScale, bool takaEtu, bool raBool)
    {
        xScale = floor.transform.localScale.x;  //Lattian keski x akseli
        zScale = floor.transform.localScale.z;    //lattian keski z akseli
        float yScale2 = floor.transform.localScale.y;
        //Jaetaan seinät toisille puolilleen
        if (takaEtu == true)
        {
            x = x - (xScale / 2);
            z = z - (zScale / 2);
        }
        else
        {
            x = x + (xScale / 2);
            z = z + (zScale / 2);
        }
        GameObject newWall = Instantiate(wall, new Vector3(x, y, z), Quaternion.Euler(0, 90, 0));
        newWall.tag = "a";
        //muutetaan seinän pituus lattian pituudeksi
        temp = newWall.gameObject.transform.localScale;
        temp.x = zScale;
        newWall.gameObject.transform.localScale = temp;

        //Seinät alkavat lattian korkeudesta
        yScale = newWall.transform.localScale.y;
        y = y + (yScale / 2) - (yScale2 / 2);

        //Sijoitetaan oikeaan kohtaan
        temp = newWall.gameObject.transform.position;
        if (takaEtu == true)
        {
            temp.z += zScale / 2;
            wallList[2][0] = newWall;
            newWall.gameObject.name = "East";
        }
        else
        {
            temp.z -= zScale / 2;
            wallList[3][0] = newWall;
            newWall.gameObject.name = "West";
        }
        temp.y = y;
        newWall.gameObject.transform.position = temp;

        if (raBool == true)  //luodaan ovi, jos on arvottu niin
        {
            CreateDoor2(floor, newWall, x, z, y, takaEtu);
        }
    }

    int CreateDoor(GameObject floor, GameObject wall2, float x, float z, float y, bool otherSide)
    {
        int suunta = 0;
        if (otherSide == true)
        {
            suunta = 0;
        }
        else
        {
            suunta = 1;
        }
        Vector3 pos = wall2.gameObject.transform.position;
        float pituus = door.gameObject.GetComponentInChildren<Transform>().localScale.x;
       
        float rand = Random.Range(3, (wall2.transform.localScale.x - 3));

        //otetaan talteen mitä jää seinän loppu osuudesta
        float lenghtLeft = wall2.transform.localScale.x;

        //Muutetaan koko
        temp = wall2.gameObject.transform.localScale;
        temp.x = rand;
        wall2.transform.localScale = temp;

        lenghtLeft -= (wall2.transform.localScale.x + doorWide); //Otetaan myös oven leveys mukaan

        float copyPos = 0f;  //seinästä toisen osan jäävä palan aloitus kohta


        Vector3 fp = floor.transform.position;  //uuden lattian sijainti
        if (otherSide == true)
        {
            copyPos = x + floor.transform.localScale.x;

            //Lattia oikealle puolelle
            float fpz = fp.z - floor.transform.localScale.z;
        }
        else
        {
            copyPos = x - floor.transform.localScale.x;

            //Lattia oikealle puolelle
            float fpz = fp.z + floor.transform.localScale.z;
        }

        GameObject copy = Instantiate(wall2, new Vector3(copyPos, y, z), Quaternion.identity); //tehdään uusi seinä
        copy.tag = "a";
        //muutetaan toisen oven pituus
        temp = copy.gameObject.transform.localScale;
        temp.x = lenghtLeft;
        copy.gameObject.transform.localScale = temp;

        //Laitetaan uuteen sijaintiin
        temp = wall2.transform.position;
        if (otherSide == true)
        {
            temp.x = x + (wall2.transform.localScale.x / 2);
        }
        else
        {
            temp.x = x - (wall2.transform.localScale.x / 2);
        }
        wall2.transform.position = temp;

        //laitetaan kopio uuteen sijaintiin
        temp = copy.transform.position;
        if (otherSide == true)
        {
            temp.x -= (copy.transform.localScale.x / 2);
            wallList[0][1] = copy;
        }
        else
        {
            temp.x += (copy.transform.localScale.x / 2);
            wallList[1][1] = copy;
        }
        copy.transform.position = temp;

        return suunta;
    }


    int CreateDoor2(GameObject floor, GameObject wall2, float x, float z, float y, bool otherSide)
    {
        int suunta = 0;
        if (otherSide == true)
        {
            suunta = 2;
        }
        else
        {
            suunta = 3;
        }
        float pituus = door.gameObject.GetComponentInChildren<Transform>().localScale.x;
        float rand = Random.Range(3, (wall2.transform.localScale.x - 3));

        //otetaan talteen mitä jää seinän loppu osuudesta
        float lenghtLeft = wall2.transform.localScale.x;

        //Muutetaan koko
        temp = wall2.gameObject.transform.localScale;
        temp.x = rand;
        wall2.transform.localScale = temp;

        lenghtLeft -= (wall2.transform.localScale.x + doorWide); //Otetaan myös oven leveys mukaan

        float copyPos = 0f;  //seinästä toisen osan jäävä palan aloitus kohta

        Vector3 fp = floor.transform.position;  //uuden lattian sijainti
        if (otherSide == true)
        {
            copyPos = z + floor.transform.localScale.z;

            //lattia
            float fpx = fp.x + floor.transform.localScale.x;
        }
        else
        {
            copyPos = z - floor.transform.localScale.z;

            //Lattia oikealle puolelle
            float fpx = fp.x - floor.transform.localScale.x;
        }

        GameObject copy = Instantiate(wall2, new Vector3(x, y, copyPos), Quaternion.Euler(0, 90, 0)); //tehdään uusi seinä
        //tagi on vain testaukseen, jotta voidaan nopeasti poistaa kaikki ja luoda uusiksi
        copy.tag = "a";  
        //muutetaan toisen oven pituus
        temp = copy.gameObject.transform.localScale;
        temp.x = lenghtLeft;
        copy.gameObject.transform.localScale = temp;

        //Laitetaan uuteen sijaintiin
        temp = wall2.transform.position;
        if (otherSide == true)
        {
            temp.z = z + (wall2.transform.localScale.x / 2);
        }
        else
        {
            temp.z = z - (wall2.transform.localScale.x / 2);
        }
        wall2.transform.position = temp;

        //laitetaan kopio uuteen sijaintiin
        temp = copy.transform.position;
        if (otherSide == true)
        {
            temp.z -= (copy.transform.localScale.x / 2);
            wallList[2][1] = copy;
        }
        else
        {
            temp.z += (copy.transform.localScale.x / 2);
            wallList[3][1] = copy;
        }
        copy.transform.position = temp;
        return suunta;
    }
}
