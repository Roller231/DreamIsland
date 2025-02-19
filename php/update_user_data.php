<?php
header("Access-Control-Allow-Origin: *"); 
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");

header("Content-Type: application/json");
require_once 'db.php';

$required = ['id', 'pirateCoins', 'monkeyCoins', 'isMale', 'skinId', 'isFirstGame'];
foreach ($required as $field) {
    if (!isset($_POST[$field])) {
        echo json_encode(["success" => false, "error" => "Missing parameter: $field"]);
        exit;
    }
}

$id = $_POST["id"];
$pirateCoins = $_POST["pirateCoins"];
$monkeyCoins = $_POST["monkeyCoins"];
$isMale = $_POST["isMale"];
$skinId = $_POST["skinId"];
$isFirstGame = $_POST["isFirstGame"];

try {
    $stmt = $pdo->prepare("UPDATE users 
                           SET pirateCoins = :pirateCoins, monkeyCoins = :monkeyCoins, 
                               isMale = :isMale, skinId = :skinId, isFirstGame = :isFirstGame 
                           WHERE id = :id");
    $stmt->bindParam(":pirateCoins", $pirateCoins);
    $stmt->bindParam(":monkeyCoins", $monkeyCoins);
    $stmt->bindParam(":isMale", $isMale);
    $stmt->bindParam(":skinId", $skinId);
    $stmt->bindParam(":isFirstGame", $isFirstGame);
    $stmt->bindParam(":id", $id);
    $stmt->execute();

    if ($stmt->rowCount() > 0) {
        echo json_encode(["success" => true, "message" => "Данные обновлены"]);
    } else {
        echo json_encode(["success" => false, "error" => "Пользователь не найден или данные не изменились"]);
    }
} catch (PDOException $e) {
    echo json_encode(["success" => false, "error" => $e->getMessage()]);
}
?>
