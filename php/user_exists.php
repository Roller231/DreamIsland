<?php
header("Access-Control-Allow-Origin: *"); 
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");

header("Content-Type: application/json");
require_once 'db.php'; // Файл подключения к базе данных

if (!isset($_GET["id"])) {
    echo json_encode(["exists" => false, "error" => "Missing user ID"]);
    exit;
}

$id = $_GET["id"];

try {
    $stmt = $pdo->prepare("SELECT COUNT(*) as count FROM users WHERE id = :id");
    $stmt->bindParam(":id", $id);
    $stmt->execute();
    $result = $stmt->fetch(PDO::FETCH_ASSOC);
    $exists = ($result["count"] > 0);
    echo json_encode(["exists" => $exists]);
} catch (PDOException $e) {
    echo json_encode(["exists" => false, "error" => $e->getMessage()]);
}
?>
