from fastapi import FastAPI
import http.client
import json


app = FastAPI()

@app.get("/")
def read_root():
    return {"Hello": "World", "Status": "API läuft!"}

@app.get("/items/{item_id}")
def read_item(item_id: int):
    conn = http.client.HTTPConnection("backend", 5000)
    conn.request("GET", "/movies/0/50")
    response = conn.getresponse()
    return response.read().decode()
    #return {"item_id": item_id, "message": "Dies ist ein dynamischer Pfad"}


@app.get("/movies/{c}/{s}")
def movies(c: int, s: int):
    return get_movies(c, s)

def get_movies(c=None, s=None):
    conn=http.client.HTTPConnection("backend",5000); conn.request("GET",f"/movies/{c or 0}/{s or 50}")
    return [e["movie"] for e in json.loads(conn.getresponse().read())["movies"]["data"] if e["error"] is None]
