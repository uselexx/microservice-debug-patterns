from fastapi import FastAPI
import http.client
import json


app = FastAPI()
#BASE_URL = 'localhost'
BASE_URL = 'backend'

@app.get("/movies/{c}/{s}")
def get_movies(c: int, s: int):
    conn = http.client.HTTPConnection(BASE_URL, 5000)
    conn.request("GET", f"/movies/{c or 0}/{s or 50}")
    return [e["movie"] for e in json.loads(conn.getresponse().read())["movies"]["data"] if e["error"] is None]

@app.get("/swipes/{user_id}")
def get_swipe_history_for_user(user_id):
    conn = http.client.HTTPConnection(BASE_URL, 5000)
    conn.request("GET", f"/swipes/{user_id}")
    return json.loads(conn.getresponse().read())["swipes"]


@app.get("/debug_get_user_recommendation_list/{user_id}/{c}/{s}")
def get_user_recommendation_list(user_id, c=None, s=None):
    # 1️⃣ fetch movies from inventory
    movies_list = get_movies(c, s)
    # 2️⃣ fetch user swipes
    user_swipes = get_swipe_history_for_user(user_id)
    # 3️⃣ filter out movies already swiped by the user
    swiped_ids = {s["movieId"] for s in user_swipes}
    return [m for m in movies_list if m["id"] not in swiped_ids]

@app.get("/debug_get_group_list_for_user/{user_id}/{c}/{s}")
def movies_group_for_user(groupname, user_id):
    # 1️⃣ load group members
    with open("groups.json", "r") as f:
        groups = json.load(f)
    members = groups.get(groupname, [])
    if user_id not in members:
        members.append(user_id)

    # 2️⃣ fetch swipes for all members
    all_swipes = {uid: get_swipe_history_for_user(uid) for uid in members}

    # 3️⃣ convert to sets of liked movie IDs
    liked_sets = {uid: {s["movieId"] for s in swipes if s["isLiked"]} for uid, swipes in all_swipes.items()}

    # 4️⃣ intersection of liked movies across all members
    intersection = set.intersection(*liked_sets.values()) if liked_sets else set()
    if intersection:
        return list(intersection)  # all liked by everyone

    # 5️⃣ else, return movies from other members that user has not seen
    user_seen_ids = {s["movieId"] for s in all_swipes[user_id]}
    other_movies = set()
    for uid, sids in liked_sets.items():
        if uid != user_id:
            other_movies.update(sids)
    return list(other_movies - user_seen_ids)

