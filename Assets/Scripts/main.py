from fastapi import FastAPI
from pydantic import BaseModel
import psycopg2
import os
from dotenv import load_dotenv

load_dotenv()

app = FastAPI()

DB_URL = os.getenv("DATABASE_URL", "postgresql://postgres:102030@localhost:5432/postgres")

def get_connection():
    return psycopg2.connect(DB_URL)

class Score(BaseModel):
    playerName: str
    score: int

@app.get("/scores/top")
def get_top_scores(limit: int = 10):
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT player_name, score FROM scores ORDER BY score DESC LIMIT %s", (limit,))
    result = [{"playerName": row[0], "score": row[1]} for row in cur.fetchall()]
    cur.close()
    conn.close()
    return {"scores": result} 

@app.post("/scores")
def submit_score(score: Score):
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT score FROM scores WHERE player_name = %s", (score.playerName,))
    row = cur.fetchone()

    if row:
        if score.score > row[0]:
            cur.execute("UPDATE scores SET score = %s WHERE player_name = %s", (score.score, score.playerName))
    else:
        cur.execute("INSERT INTO scores (player_name, score) VALUES (%s, %s)", (score.playerName, score.score))

    conn.commit()
    cur.close()
    conn.close()

    return {"message": "Score submitted"}

@app.get("/ping")
def ping():
    return {"status": "ok"}
