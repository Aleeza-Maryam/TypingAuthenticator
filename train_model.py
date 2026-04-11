import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import accuracy_score, classification_report
import pickle
import os

def train_model():
    print("=== Training Typing Authentication Model ===\n")

    if not os.path.exists("data/typing_data.csv"):
        print("No data found!")
        return

    df = pd.read_csv("data/typing_data.csv")
    
    # Sirf last data rakho — duplicates hata do
    df = df.drop_duplicates()

    print(f"Total samples: {len(df)}")
    print(f"Users: {df['username'].unique()}")
    print(f"Samples per user:\n{df['username'].value_counts()}\n")

    feature_cols = [
        'total_time', 'avg_pause', 'max_pause',
        'min_pause', 'typing_speed', 'error_count',
        'error_rate', 'rhythm_variance'
    ]

    X = df[feature_cols]
    y = df['username']

    le = LabelEncoder()
    y_encoded = le.fit_transform(y)

    print(f"All users in model: {le.classes_}\n")

    X_train, X_test, y_train, y_test = train_test_split(
        X, y_encoded, test_size=0.2, random_state=42, stratify=y_encoded
    )

    model = RandomForestClassifier(
        n_estimators=200,
        max_depth=15,
        random_state=42
    )
    model.fit(X_train, y_train)

    y_pred = model.predict(X_test)
    accuracy = accuracy_score(y_test, y_pred)

    print(f"✅ Model trained!")
    print(f"Accuracy: {accuracy * 100:.2f}%")
    print(f"\nDetailed Report:")
    print(classification_report(y_test, y_pred,
          target_names=le.classes_))

    os.makedirs("models", exist_ok=True)

    with open("models/typing_model.pkl", "wb") as f:
        pickle.dump(model, f)

    with open("models/label_encoder.pkl", "wb") as f:
        pickle.dump(le, f)

    print("\n✅ Model saved!")
    print(f"Users in model: {le.classes_}")

if __name__ == "__main__":
    train_model()