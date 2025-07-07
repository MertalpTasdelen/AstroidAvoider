from fastapi import FastAPI
from pydantic import BaseModel
from typing import List
import psycopg2
import os
from dotenv import load_dotenv

load_dotenv()

app = FastAPI()

DB_URL = os.getenv("DATABASE_URL", "postgresql://user:password@localhost:5432/dbname")

def get_connection():
    return psycopg2.connect(DB_URL)

class Score(BaseModel):
    user: str
    score: int

class Achievement(BaseModel):
    id: str
    title: str
    description: str
    target_amount: int
    type: str
    is_daily: bool

@app.get("/achievements/global", response_model=List[Achievement])
def get_global_achievements():
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT id, title, description, target_amount, type, is_daily FROM achievements")
    rows = cur.fetchall()
    cur.close()
    conn.close()
    return [
        Achievement(
            id=row[0], title=row[1], description=row[2],
            target_amount=row[3], type=row[4], is_daily=row[5]
        ) for row in rows
    ]

class PlayerAchievement(BaseModel):
    achievement_id: str
    current_amount: int
    is_completed: bool

@app.get("/achievements/user/{username}", response_model=List[PlayerAchievement])
def get_user_achievements(username: str):
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("""
        SELECT achievement_id, current_amount, is_completed
        FROM player_achievements
        WHERE player_name = %s
    """, (username,))
    rows = cur.fetchall()
    cur.close()
    conn.close()
    return [PlayerAchievement(achievement_id=row[0], current_amount=row[1], is_completed=row[2]) for row in rows]

class AchievementProgressRequest(BaseModel):
    username: str
    achievement_id: str
    progress: int

@app.post("/achievements/progress")
def update_achievement_progress(data: AchievementProgressRequest):
    conn = get_connection()
    cur = conn.cursor()

    cur.execute("SELECT target_amount FROM achievements WHERE id = %s", (data.achievement_id,))
    result = cur.fetchone()
    if result is None:
        cur.close()
        conn.close()
        return {"error": "Achievement not found"}

    target = result[0]

    cur.execute("""
        SELECT current_amount, is_completed
        FROM player_achievements
        WHERE player_name = %s AND achievement_id = %s
    """, (data.username, data.achievement_id))
    row = cur.fetchone()

    if row:
        current_amount, is_completed = row
        if not is_completed:
            new_amount = min(current_amount + data.progress, target)
            new_completed = new_amount >= target
            cur.execute("""
                UPDATE player_achievements
                SET current_amount = %s, is_completed = %s
                WHERE player_name = %s AND achievement_id = %s
            """, (new_amount, new_completed, data.username, data.achievement_id))
    else:
        new_amount = min(data.progress, target)
        new_completed = new_amount >= target
        cur.execute("""
            INSERT INTO player_achievements (player_name, achievement_id, current_amount, is_completed)
            VALUES (%s, %s, %s, %s)
        """, (data.username, data.achievement_id, new_amount, new_completed))

    conn.commit()
    cur.close()
    conn.close()

    return {"message": "Progress updated", "completed": new_completed}

@app.get("/scores/top")
def get_top_scores(limit: int = 10):
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT player_name, score FROM scores ORDER BY score DESC LIMIT %s", (limit,))
    result = [{"user": row[0], "score": row[1]} for row in cur.fetchall()]
    cur.close()
    conn.close()
    return {"scores": result}

@app.post("/scores")
def submit_score(score: Score):
    conn = get_connection()
    cur = conn.cursor()
    cur.execute("SELECT score FROM scores WHERE player_name = %s", (score.user,))
    row = cur.fetchone()

    if row:
        if score.score > row[0]:
            cur.execute("UPDATE scores SET score = %s WHERE player_name = %s", (score.score, score.user))
    else:
        cur.execute("INSERT INTO scores (player_name, score) VALUES (%s, %s)", (score.user, score.score))

    conn.commit()
    cur.close()
    conn.close()

    return {"message": "Score submitted"}

@app.get("/ping")
def ping():
    return {"status": "ok"}
