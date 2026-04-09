import time
import csv
import os
import msvcrt

PARAGRAPH = "the quick brown fox jumps over the lazy dog"

def collect_typing_data(username):
    print(f"\nType this paragraph exactly:")
    print(f"\n>>> {PARAGRAPH}\n")
    print("Press ENTER when ready...")
    input()

    print("START TYPING:\n")

    pauses = []
    errors = 0
    typed = ""
    prev_time = time.time()

    while True:
        ch = msvcrt.getwch()

        if ch == '\r':  # Enter
            break
        elif ch == '\x08':  # Backspace
            if typed:
                typed = typed[:-1]
                errors += 1
            continue
        elif ch == '\x03':  # Ctrl+C
            break

        now = time.time()
        pauses.append(round(now - prev_time, 4))
        prev_time = now
        typed += ch
        print(f"\r{typed}", end='', flush=True)

    total_time = sum(pauses)
    avg_pause  = round(sum(pauses) / len(pauses), 4) if pauses else 0
    variance   = round(
        sum((p - avg_pause) ** 2 for p in pauses) / len(pauses), 6
    ) if pauses else 0

    return {
        'username':        username,
        'total_time':      round(total_time, 2),
        'avg_pause':       avg_pause,
        'max_pause':       round(max(pauses), 4) if pauses else 0,
        'min_pause':       round(min(pauses), 4) if pauses else 0,
        'typing_speed':    round(len(typed) / total_time, 2) if total_time > 0 else 0,
        'error_count':     errors,
        'error_rate':      round(errors / len(PARAGRAPH), 4),
        'rhythm_variance': variance
    }

def save_data(features, filename="data/typing_data.csv"):
    os.makedirs("data", exist_ok=True)
    file_exists = os.path.exists(filename)

    with open(filename, 'a', newline='') as f:
        writer = csv.DictWriter(f, fieldnames=features.keys())
        if not file_exists:
            writer.writeheader()
        writer.writerow(features)

    print(f"\n✅ Session saved!")

def main():
    username = input("Enter your username: ").strip()
    sessions = 8  # 8 baar type karwao

    print(f"\n{sessions} sessions hongi — thoda rest milega beech mein.")

    for i in range(sessions):
        print(f"\n{'='*45}")
        print(f"Session {i+1} of {sessions}")
        print(f"{'='*45}")
        data = collect_typing_data(username)
        save_data(data)

        print(f"Speed: {data['typing_speed']} chars/sec | Errors: {data['error_count']}")

        if i < sessions - 1:
            print("\nRest 2 seconds...")
            time.sleep(2)

    print(f"\n🎉 Done! {sessions} sessions recorded for '{username}'")

if __name__ == "__main__":
    main()