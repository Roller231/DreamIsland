<?php
header("Access-Control-Allow-Origin: *"); 
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");

header("Content-Type: application/json");
require_once 'db.php';

if (!isset($_GET["id"])) {
    echo json_encode(["success" => false, "error" => "Missing user ID"]);
    exit;
}

$id = $_GET["id"];

try {
    $stmt = $pdo->prepare("SELECT pirateCoins, monkeyCoins, isMale, skinId, isFirstGame 
                           FROM users WHERE id = :id LIMIT 1");
    $stmt->bindParam(":id", $id);
    $stmt->execute();
    $userData = $stmt->fetch(PDO::FETCH_ASSOC);
    
    if ($userData) {
        echo json_encode(["success" => true, "data" => $userData]);
    } else {
        echo json_encode(["success" => false, "error" => "Пользователь не найден"]);
    }
} catch (PDOException $e) {
    echo json_encode(["success" => false, "error" => $e->getMessage()]);
}
?>
