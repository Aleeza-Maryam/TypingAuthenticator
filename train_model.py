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
    
    # Data load karo
    if not os.path.exists("data/typing_data.csv"):
        print("❌ No data found! Run collect_data.py first.")
        return
    
    df = pd.read_csv("data/typing_data.csv")
    
    print(f"Total samples: {len(df)}")
    print(f"Users: {df['username'].unique()}")
    print(f"Samples per user:\n{df['username'].value_counts()}\n")
    
    # Features aur labels alag karo
    feature_cols = [
        'total_time', 'avg_pause', 'max_pause', 
        'min_pause', 'typing_speed', 'error_count',
        'error_rate', 'rhythm_variance'
    ]
    
    X = df[feature_cols]
    y = df['username']
    
    # Label encode karo
    le = LabelEncoder()
    y_encoded = le.fit_transform(y)
    
    # Train/test split
    X_train, X_test, y_train, y_test = train_test_split(
        X, y_encoded, test_size=0.2, random_state=42
    )
    
    # Model banao aur train karo
    print("Training Random Forest model...")
    model = RandomForestClassifier(
        n_estimators=100,
        max_depth=10,
        random_state=42
    )
    model.fit(X_train, y_train)
    
    # Accuracy check karo
    y_pred = model.predict(X_test)
    accuracy = accuracy_score(y_test, y_pred)
    
    print(f"\n✅ Model trained!")
    print(f"Accuracy: {accuracy * 100:.2f}%")
    print(f"\nDetailed Report:")
    print(classification_report(y_test, y_pred, 
          target_names=le.classes_))
    
    # Feature importance
    print("\nFeature Importance:")
    for feat, imp in sorted(
        zip(feature_cols, model.feature_importances_),
        key=lambda x: x[1], reverse=True
    ):
        print(f"  {feat}: {imp:.4f}")
    
    # Model save karo
    os.makedirs("models", exist_ok=True)
    
    with open("models/typing_model.pkl", "wb") as f:
        pickle.dump(model, f)
    
    with open("models/label_encoder.pkl", "wb") as f:
        pickle.dump(le, f)
    
    print("\n✅ Model saved to models/typing_model.pkl")

if __name__ == "__main__":
    train_model()