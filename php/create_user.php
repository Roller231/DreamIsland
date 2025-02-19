<?php
header("Access-Control-Allow-Origin: *"); 
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");

header("Content-Type: application/json");
require_once 'db.php';

if (!isset($_POST["id"])) {
    echo json_encode(["success" => false, "error" => "Missing user ID"]);
    exit;
}

$id = $_POST["id"];

try {
    $stmt = $pdo->prepare("INSERT INTO users (id, pirateCoins, monkeyCoins, isMale, skinId, isFirstGame) 
                           VALUES (:id, 0, 0, 1, 0, 1)");
    $stmt->bindParam(":id", $id);
    $stmt->execute();
    echo json_encode(["success" => true, "message" => "Пользователь создан"]);
} catch (PDOException $e) {
    echo json_encode(["success" => false, "error" => $e->getMessage()]);
}
?>
